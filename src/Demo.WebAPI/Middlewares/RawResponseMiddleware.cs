namespace Demo.WebAPI.Middlewares
{
    public class RawResponseMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public RawResponseMiddleware(ILogger<RawResponseMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestId = context.Items["RequestId"]?.ToString() ?? "";
            var originalBody = context.Response.Body!;
            using (var memoryStream = new MemoryStream())
            {
                context.Response.Body = memoryStream;
                try
                {
                    await _next(context);
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    memoryStream.Position = 0;
                    var responseBody = new StreamReader(memoryStream).ReadToEnd();
                    memoryStream.Position = 0;
                    await memoryStream.CopyToAsync(originalBody);
                    context.Response.Body = originalBody;
                    //_logger.LogInfo(requestId, responseBody);
                }
            }
        }


    }
}
