using Demo.Shared.enums;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.WebAPI.Authorizations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class DemoAuthorizeAttribute : AuthorizeAttribute
    {
        public Permission[] Permissions { get; set; }
        public DemoAuthorizeAttribute(params Permission[] permissions)
        {
            this.Permissions = permissions;
        }
    }
}
