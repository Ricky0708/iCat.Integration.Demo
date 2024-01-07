using Demo.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Repositories.Interfaces
{
    public interface IUserRepository
    {
        UserDao? GetUserById(int userId);
        IEnumerable<UserPermissionDao>? GetPermissionsById(int userId);
    }
}
