using Demo.Repositories.Implements;
using Demo.Repositories.Interfaces;
using Demo.Services.Implements;
using Demo.Services.Interfaces;
using Demo.Shared.Constants;
using Demo.Shared.enums;
using Demo.WebAPI.Middlewares;
using Demo.WebAPI.Models;
using Demo.WebAPI.Providers;
using iCat.Authorization;
using iCat.Authorization.Extensions;
using iCat.Cache.Interfaces;
using iCat.Crypto.Interfaces;
using iCat.DB.Client.Extension.Web;
using iCat.DB.Client.Factory.Interfaces;
using iCat.DB.Client.Models;
using iCat.Localization.Extension.Web;
using iCat.Localization.Extensions;
using iCat.Localization.Models;
using iCat.RabbitMQ.Implements;
using iCat.RabbitMQ.Interfaces;
using iCat.Token.Implements;
using iCat.Token.Interfaces;
using iCat.Token.JWT;
using iCat.Worker.Implements;
using iCat.Worker.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using System.Globalization;
using System.Text;
namespace Demo.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var a = CultureInfo.GetCultures(CultureTypes.AllCultures);
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            // Add services to the container.

            ConfigureDBClient(services);
            ConfigureToken(services);
            ConfigureAuthorization(services);
            ConfigureLocalization(services);
            ConfigureCache(services);
            ConfigureCrypto(services);
            ConfigureRabbitMQ(services);

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();

            services.AddControllers();


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseMiddleware<GuidMiddleware>();
            app.UseMiddleware<PerformanceMiddleware>();
            app.UseMiddleware<RawReqeustMiddleware>();
            app.UseMiddleware<RawResponseMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // db, worker
            UpdateDBClientTask(app);

            // localizaion static method
            app.UseiCatLocalizationExtension();

            // MQ, todo: need to set RabbitMQ connection first
            //PublishAndSubscriber(app);

            app.Run();
        }

        #region DBClient

        private static IServiceCollection ConfigureDBClient(IServiceCollection services)
        {
            // for single database, you can use this method and use IUnitOfWork/IConnection directly for DI
            //services
            //    .AddDBClient(new DBClient(new DBClientInfo("", "your connection string")));

            services
               .AddDBClientFactory(s => new CustomDBClientProvider());
            return services;
        }

        private static void UpdateDBClientTask(WebApplication app)
        {
            // Update db clients every 5 mins
            var updateDBClientTask = new IntervalTask(
                (CustomDBClientProvider)app.Services.GetRequiredService<IDBClientProvider>(),
                1 * 1000 * 60 * 5,
                new IntervalTaskOption
                {
                    IsExecuteWhenStart = true,
                    RetryInterval = 3000,
                    RetryTimes = 5
                });
            updateDBClientTask.Start();
        }

        #endregion

        #region Token

        private static IServiceCollection ConfigureToken(IServiceCollection services)
        {
            var secretKey = "897&*()&@ljlcsfg7w7dflsdkek4f984";
            services
                .AddSingleton<ITokenGenerator>(p => new TokenGenerator(secretKey)) // JWT
                .AddSingleton<ITokenValidator>(p => new TokenValidator(secretKey)) // JWT
                .AddSingleton<ITokenService<TokenDataModel>, TokenService<TokenDataModel>>(); // JWT
            return services;
        }

        #endregion

        #region Authorization

        private static IServiceCollection ConfigureAuthorization(IServiceCollection services)
        {
            var secretKey = "897&*()&@ljlcsfg7w7dflsdkek4f984";
            services.AddScoped<RequestManager>()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<IAuthorizationMiddlewareResultHandler, DemoAuthorizationMiddlewareResultHandler>()
                .AddAuthorizationPermission(typeof(MyFunction))
                .AddAuthorization(options =>
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme, "Bearer")
                        .AddAuthorizationPermissionRequirment()
                        .RequireAuthenticatedUser()
                        .Build();

                })
                .AddAuthentication()
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.ForwardChallenge = "Bearer";
                })
                .AddJwtBearer("Bearer", option =>
                {
                    option.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents();
                    option.Events.OnChallenge = async (context) =>
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("You are not authorized! (or some other custom message)");
                    };

                    option.TokenValidationParameters = new TokenValidationParameters()
                    {
                        RequireExpirationTime = true,
                        ClockSkew = new TimeSpan(0, 0, 0),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = false,
                        ValidAudience = "Ricky",
                        ValidIssuer = "Fz",
                        ValidateLifetime = false,
                    };
                });
            return services;
        }

        #endregion

        #region Localization

        private static IServiceCollection ConfigureLocalization(IServiceCollection services)
        {
            services
            .AddControllersWithViews()
            .AddViewLocalization() // Localiztion in Razor View
            .AddDataAnnotationsLocalization(); // Localization in Model Validation

            // Configure cultureInfo from request and support list
            services.AddRequestLocalizationOptions(new System.Globalization.CultureInfo[] {
                new System.Globalization.CultureInfo("en-US"),
                new System.Globalization.CultureInfo("zh-TW"),
            }, "LangCode");  // key in Route/QueryString/Cookie

            // Configure localization data
            services.AddiCatLocalizationeService(new List<LocalizationMapping> {
                new LocalizationMapping {
                    CultureName = "en-US",
                    LanguageData = new Dictionary<string, string>{
                        {"UserNotFound", "User Id {#UserId} is not found" },
                    }
                },
                new LocalizationMapping {
                    CultureName = "zh-TW",
                    LanguageData = new Dictionary<string, string>{
                        {"UserNotFound", "找不到這個 {#UserId} Id" },
                    }
                }
            }, new Options { EnableKeyNotFoundException = false });

            return services;
        }

        #endregion

        #region Cache

        private static IServiceCollection ConfigureCache(IServiceCollection services)
        {
            services.AddDistributedMemoryCache(); // inject memory cache into IDistributedCache
            services.AddSingleton<ICache, iCat.Cache.Implements.Cache>(); // IDistributedCache adapter
            return services;
        }

        #endregion

        #region Crypto

        private static IServiceCollection ConfigureCrypto(IServiceCollection services)
        {
            var rsaPrivateKey = "fill your RSA private key";
            var rsaPublicKey = "fill your RSA public key";
            services.AddSingleton<ICryptor>(p => new iCat.Crypto.Implements.Cryptors.AES("AES", "12345678"));
            services.AddSingleton<ICryptor>(p => new iCat.Crypto.Implements.Cryptors.DES("DES", "12345678"));
            services.AddSingleton<ICryptor>(p => new iCat.Crypto.Implements.Cryptors.RSA("RSA", rsaPublicKey, rsaPrivateKey));
            services.AddSingleton<IHasher>(p => new iCat.Crypto.Implements.Hashes.Hasher("Hash", "12345678"));
            return services;
        }

        #endregion

        #region RabbitMQ

        private static IServiceCollection ConfigureRabbitMQ(IServiceCollection services)
        {

            services.AddSingleton<IConnection>(s =>
            {
                // // TODO: rabbit mq connection
                var factory = new RabbitMQ.Client.ConnectionFactory();
                var connection = factory.CreateConnection();
                return connection;
            });
            services.AddScoped<IPublisher>(p => new Publisher(p.GetRequiredService<IConnection>(), "Master", "Demo"));
            services.AddSingleton<ISubscriber>(p => new Subscriber(p.GetRequiredService<IConnection>(), "Master", "Demo"));
            return services;
        }

        private static void PublishAndSubscriber(WebApplication app)
        {
            var subscriber = app.Services.GetRequiredService<ISubscriber>();
            subscriber.Subscribe<DemoRabbitMQModel>("DemoQueue", true, data =>
            {
                // process your data
                return true; // if not return true or false, it will auto ack 
            });

            var publisher = app.Services.GetRequiredService<IPublisher>();
            publisher.SendAsync(new DemoRabbitMQModel
            {
                Address = "Address",
                Company = "CompanyId",
                TraceId = "RequestId"
            });
        }

        #endregion
    }
}
