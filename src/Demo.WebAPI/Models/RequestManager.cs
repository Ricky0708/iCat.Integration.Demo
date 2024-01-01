using Demo.Services.Interfaces;
using iCat.DB.Client.Factory.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Demo.WebAPI.Models
{
    public class RequestManager
    {
        private HttpContext _context;
        private readonly IUserService _userService;

        public string? RequestId => _context.Items["RequestId"]?.ToString();
        public string? UserName => _context.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value;
        public int? UserId => int.TryParse(_context.User.Claims.FirstOrDefault(p => p.Type == "UserId")?.Value, out var _userId) ? _userId : null;

        public RequestManager(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService
            )
        {
            _context = httpContextAccessor.HttpContext!;
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }
    }
}
