using Demo.Shared.enums;
using Demo.WebAPI.Authorizations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Demo.WebAPI.Authorizations
{
    public class DemoHandler : AuthorizationHandler<DemoRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DemoRequirement requirement)
        {

            if (context.User.Identity?.IsAuthenticated ?? false)
            {
                if (context.Resource is HttpContext httpContext)
                {
                    var endpoint = httpContext.GetEndpoint();
                    var actionDescriptor = endpoint!.Metadata.GetMetadata<ControllerActionDescriptor>();

                    var permissionAttrs = actionDescriptor!
                        .MethodInfo.CustomAttributes.Where(p => p.AttributeType == typeof(DemoAuthorizeAttribute));
                    var permissionNeeds = permissionAttrs.SelectMany(p => p.ConstructorArguments.SelectMany(x => (ReadOnlyCollection<CustomAttributeTypedArgument>)x.Value!).Select(n => (DemoPermission)n.Value!)).ToList();
                    if (long.TryParse(context.User.Claims.FirstOrDefault(p => p.Type == "Permissions")?.Value, out var requestPermissions))
                    {
                        if (permissionNeeds.Any(p => ((DemoPermission)requestPermissions).HasFlag(p)))
                        {
                            context.Succeed(requirement);
                        }
                        else
                        {
                            context.Fail();

                        }
                    }
                    else
                    {
                        context.Fail();

                    }
                }
                else
                {
                    context.Fail();
                }

                //var number = requirement.RoleNumbers;
                //if (long.TryParse(context.User.Claims.FirstOrDefault(p => p.Type == "RoleNumber")?.Value, out var userNumber))
                //{
                //    if ((number & userNumber) > 0)
                //    {
                //        context.Succeed(requirement);
                //    }
                //    else
                //    {
                //        context.Fail();
                //    }
                //}
                //else
                //{
                //    context.Fail();
                //};
            }
            else
            {
                context.Fail();
            }
            return Task.FromResult(0);
        }
    }
}
