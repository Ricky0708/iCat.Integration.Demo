using Demo.Shared.enums;
using Demo.WebAPI.Authorizations;
using Demo.WebAPI.Models;
using iCat.Token.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using iCat.Token.Implements;
using Demo.Services.Interfaces;
using iCat.Localization.Extensions;

namespace Demo.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private const string _action = "[action]";
        private readonly ITokenService<TokenDataModel> _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly RequestManager _requestManager;

        public LoginController(
            ITokenService<TokenDataModel> tokenService,
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            RequestManager requestManager)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _requestManager = requestManager ?? throw new ArgumentNullException(nameof(requestManager));
        }

        [AllowAnonymous]
        [HttpPost(_action)]
        public async Task<IActionResult> Cookie(LoginViewModel loginViewModel)
        {
            var principal = _userService.GetUserClaimsPrincipalById(loginViewModel.UserId);
            if (principal != null)
            {
                await (_httpContextAccessor.HttpContext?.SignInAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme, principal: principal) ?? Task.CompletedTask);
                return await Task.FromResult(Ok());
            }
            else
            {
                return await Task.FromResult(BadRequest("{UserNotFound}".AddParams(new { UserId = loginViewModel.UserId }).Localize()));
            }

        }

        [AllowAnonymous]
        [HttpPost(_action)]
        public async Task<IActionResult> JWT(LoginViewModel loginViewModel)
        {
            var user = _userService.GetUserById(loginViewModel.UserId);
            if (user != null)
            {
                var result = _tokenService.GenerateToken(
                    new TokenDataModel
                    {
                        UserId = user.UserId,
                        UserName = user.UserName,
                        Permissions = user.Permissions
                    });
                return await Task.FromResult(Ok(result));
            }
            else
            {
                return await Task.FromResult(BadRequest("{UserNotFound}".AddParams(new { UserId = loginViewModel.UserId }).Localize()));
            }
        }
    }
}
