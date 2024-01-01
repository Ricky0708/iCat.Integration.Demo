using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.WebAPI.Middlewares
{
    public class DemoAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();
        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {

            // If the authorization was forbidden and the resource had a specific requirement,
            // provide a custom 404 response.
            if (authorizeResult.Forbidden)
            {
                // Return a 404 to make it appear as if the resource doesn't exist.
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync(string.Empty);
                return;
            }

            // Fall back to the default implementation.
            await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }

}
