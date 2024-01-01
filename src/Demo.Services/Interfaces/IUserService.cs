using Demo.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Services.Interfaces
{
    public interface IUserService
    {
        UserDto? GetUserById(int userId);
        ClaimsPrincipal? GetUserClaimsPrincipalById(int userId);
    }
}
