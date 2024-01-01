using iCat.Logger.Extension;

namespace Demo.WebAPI.Middlewares
{
    public class PerformanceMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public PerformanceMiddleware(ILogger<PerformanceMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestId = context.Items["RequestId"]?.ToString() ?? "";
            var location = new Uri($"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");
            var url = location.AbsoluteUri;
            await _logger.LogPerformance($"RequestId: {requestId}, {url} Done", async () => await _next(context));
        }
    }
}
