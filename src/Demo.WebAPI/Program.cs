using Demo.Shared.Constants;
using Demo.WebAPI.Middlewares;
using Demo.WebAPI.Models;
using Demo.WebAPI.Providers;
using iCat.DB.Client.Extension.Web;
using iCat.DB.Client.Factory.Interfaces;
using iCat.DB.Client.Models;
using iCat.DB.Client.MSSQL;
using iCat.Token.Implements;
using iCat.Token.Interfaces;
using iCat.Token.JWT;
using iCat.Worker.Implements;
using iCat.Worker.Models;

namespace Demo.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            // Add services to the container.

            ConfigureDBClient(services);
            ConfigureToken(services);



            services.AddControllers();


            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseMiddleware<GuidMiddleware>();
            app.UseMiddleware<PerformanceMiddleware>();
            app.UseMiddleware<RawReqeustMiddleware>();
            app.UseMiddleware<RawResponseMiddleware>();

            app.UseAuthorization();
            app.UseAuthorization();

            app.MapControllers();

            UpdateDBClientTask(app);

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


    }
}
