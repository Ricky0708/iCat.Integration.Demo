using System.Text;

namespace Demo.WebAPI.Middlewares
{
    public class RawReqeustMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public RawReqeustMiddleware(ILogger<RawReqeustMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestId = context.Items["RequestId"]?.ToString() ?? "";

            #region request
            //var actionDescriptor = context.GetEndpoint()!.Metadata.GetMetadata<ControllerActionDescriptor>();
            //var ignoreBody = actionDescriptor!.MethodInfo.CustomAttributes.Where(p => p.AttributeType == typeof(AllowUnFillProfileAttribute)).Count() != 0;

            var endpoint = new Uri($"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");
            var request = context.Request;
            var ignoreBody = (request.ContentType?.IndexOf("multipart/form-data; boundary") > -1);

            if (!request.Body.CanSeek)
            {
                request.EnableBuffering();
            }
            request.Body.Position = 0;
            var requestBodyReader = new StreamReader(request.Body, Encoding.UTF8);
            var requestBody = await requestBodyReader.ReadToEndAsync().ConfigureAwait(false);
            request.Body.Position = 0;

            var url = endpoint.AbsoluteUri;
            //_logger.LogInfo(requestId, $"Request:{endpoint},{(ignoreBody ? "ignored" : requestBody)}");
            await _next(context);
            #endregion

        }
    }
}
