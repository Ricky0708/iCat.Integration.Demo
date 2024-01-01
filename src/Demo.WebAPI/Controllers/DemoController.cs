using Demo.Services.Interfaces;
using Demo.Shared.enums;
using Demo.WebAPI.Authorizations;
using Demo.WebAPI.Models;
using iCat.Localization.Extensions;
using iCat.Token.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iCat.Cache.Interfaces;
using iCat.Crypto.Interfaces;

namespace Demo.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DemoController : ControllerBase
    {
        private const string _action = "[action]";
        private readonly IUserService _userService;
        private readonly RequestManager _requestManager;
        private readonly ICache _cache;
        private readonly IEnumerable<ICryptor> _cryptors;
        private readonly IHasher _hasher;

        public DemoController(
            IUserService userService,
            RequestManager requestManager,
            ICache cache,
            IEnumerable<ICryptor> cryptors,
            IHasher hasher)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _requestManager = requestManager ?? throw new ArgumentNullException(nameof(requestManager));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _cryptors = cryptors ?? throw new ArgumentNullException(nameof(cryptors));
            _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        }

        [DemoAuthorize(Permission.Read)]
        [HttpGet("[action]")]
        public async Task<IActionResult> AuthorizationData()
        {
            var id = _requestManager.UserId;
            var name = _requestManager.UserName;
            var permission = _requestManager.Permissions;
            return await Task.FromResult(Ok(new
            {
                RequestId = _requestManager.RequestId,
                Id = id,
                Name = name,
                Permission = permission
            }));
        }

        [DemoAuthorize(Permission.Read)]
        [HttpGet("[action]/{key}")]
        public async Task<IActionResult> GetCache(string key)
        {
            return await Task.FromResult(Ok(_cache.GetString(key)));
        }

        [DemoAuthorize(Permission.Read)]
        [HttpGet("[action]/{key}/{value}")]
        public async Task<IActionResult> SetCache(string key, string value)
        {
            _cache.Set(key, value);
            return await Task.FromResult(Ok());
        }

        [DemoAuthorize(Permission.Read)]
        [HttpGet("[action]/{encryptMethod}/{plainText}")]
        public async Task<IActionResult> Encrypt(string encryptMethod, string plainText)
        {
            var cryptor = _cryptors.First(p => p.Category == encryptMethod);

            return await Task.FromResult(Ok(cryptor.Encrypt(plainText)));
        }

        [DemoAuthorize(Permission.Read)]
        [HttpGet("[action]/{encryptMethod}/{cipherText}")]
        public async Task<IActionResult> Decrypt(string encryptMethod, string cipherText)
        {
            var cryptor = _cryptors.First(p => p.Category == encryptMethod);

            return await Task.FromResult(Ok(cryptor.Decrypt(cipherText)));
        }

        [DemoAuthorize(Permission.Read)]
        [HttpGet("[action]/{plainText}")]
        public async Task<IActionResult> Hash(string plainText)
        {
            return await Task.FromResult(Ok(_hasher.SHA512(plainText)));
        }
    }
}
