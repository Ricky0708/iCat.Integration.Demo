﻿using Demo.Repositories.Implements;
using Demo.Repositories.Interfaces;
using Demo.Services.Interfaces;
using Demo.Services.Models;
using Demo.Shared.enums;
using iCat.Authorization.Models;
using iCat.Authorization.Utilities;
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
        private readonly IFunctionPermissionProvider _functionPermissionProvider;

        public UserService(IUserRepository userRepository, IFunctionPermissionProvider functionPermissionProvider)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _functionPermissionProvider = functionPermissionProvider ?? throw new ArgumentNullException(nameof(functionPermissionProvider));
        }

        public UserDto? GetUserById(int userId)
        {
            var result = _userRepository.GetUserById(userId);
            var userPermissions = _userRepository.GetPermissionsById(userId);
            if (result == null) return null;

            return new UserDto
            {
                UserId = result.UserId,
                UserName = result.UserName,
                Permissions = from p in userPermissions
                              group p by p.FunctionValue into groupValue
                              select new FunctionPermissionData
                              {
                                  FunctionValue = groupValue.Key,
                                  FunctionName = groupValue.First().FunctionName,
                                  PermissionDetails = groupValue.Select(n => new PermissionDetail { PermissionName = n.PermissionName, Permission = n.Permission }).ToList()
                              }
            };
        }

        public ClaimsPrincipal? GetUserClaimsPrincipalById(int userId)
        {
            var user = _userRepository.GetUserById(userId);
            var userPermissions = _userRepository.GetPermissionsById(userId);
            var Permissions = from p in userPermissions
                              group p by p.FunctionValue into groupValue
                              select new FunctionPermissionData
                              {
                                  FunctionValue = groupValue.Key,
                                  FunctionName = groupValue.First().FunctionName,
                                  PermissionDetails = groupValue.Select(n => new PermissionDetail { PermissionName = n.PermissionName, Permission = n.Permission }).ToList()
                              };
            if (user == null) return null;

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim("UserId", user.UserId.ToString()));
            foreach (var item in Permissions)
            {
                claims.Add(_functionPermissionProvider.GetClaimFromFunctionPermissionData(item));
            }

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            return principal;
        }

        public List<Claim>? GetUserClaimsById(int userId)
        {
            var user = _userRepository.GetUserById(userId);
            var userPermissions = _userRepository.GetPermissionsById(userId);
            var Permissions = from p in userPermissions
                              group p by p.FunctionValue into groupValue
                              select new FunctionPermissionData
                              {
                                  FunctionValue = groupValue.Key,
                                  FunctionName = groupValue.First().FunctionName,
                                  PermissionDetails = groupValue.Select(n => new PermissionDetail { PermissionName = n.PermissionName, Permission = n.Permission }).ToList()
                              };
            if (user == null) return null;

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim("UserId", user.UserId.ToString()));
            foreach (var item in Permissions)
            {
                claims.Add(_functionPermissionProvider.GetClaimFromFunctionPermissionData(item));
            }
            return claims;
        }
    }
}
