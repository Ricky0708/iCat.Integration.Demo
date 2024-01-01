namespace Demo.WebAPI.Middlewares
{
    public class GuidMiddleware
    {
        private readonly RequestDelegate _next;

        public GuidMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var guid = Guid.NewGuid();
            context.Items.Add("RequestId", guid.ToString());
            await _next(context);
        }
    }
}
