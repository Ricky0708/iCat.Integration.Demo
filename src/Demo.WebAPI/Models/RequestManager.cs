using Demo.Services.Interfaces;
using Demo.Services.Models;
using Demo.Shared.enums;
using iCat.Authorization.Models;
using iCat.Authorization.Utilities;
using iCat.DB.Client.Factory.Interfaces;
using iCat.Token.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
        private readonly ITokenService<TokenDataModel> _tokenService;
        private readonly IUserPermissionProvider _userPermissionProvider;

        public string? RequestId => _context.Items["RequestId"]?.ToString();
        public string? UserName => _context.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value;
        public int? UserId => int.TryParse(_context.User.Claims.FirstOrDefault(p => p.Type == "UserId")?.Value, out var _userId) ? _userId : null;
        public IEnumerable<FunctionData> Permissions => _userPermissionProvider.GetUserPermission();


        private Lazy<UserDto?> UserData => new(() => (UserId == null ? null : _userService.GetUserById(UserId.Value)));

        public RequestManager(
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            ITokenService<TokenDataModel> tokenService,
            IUserPermissionProvider userPermissionProvider
            )
        {
            _context = httpContextAccessor.HttpContext!;
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _userPermissionProvider = userPermissionProvider ?? throw new ArgumentNullException(nameof(userPermissionProvider));
        }
    }
}
