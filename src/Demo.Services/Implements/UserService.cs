using Demo.Repositories.Implements;
using Demo.Repositories.Interfaces;
using Demo.Services.Interfaces;
using Demo.Services.Models;
using Demo.Shared.enums;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace Demo.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public UserDto? GetUserById(int userId)
        {
            var result = _userRepository.GetUserById(userId);
            if (result == null) return null;

            return new UserDto
            {
                UserId = result.UserId,
                UserName = result.UserName,
                Permissions = new 
            };
        }

        public ClaimsPrincipal? GetUserClaimsPrincipalById(int userId)
        {
            var result = _userRepository.GetUserById(userId);
            if (result == null) return null;

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, result.UserName));
            claims.Add(new Claim("UserId", result.UserId.ToString()));
            claims.Add(new Claim("Permissions", ((long)result.Permissions).ToString()));

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            return principal;
        }
    }
}
