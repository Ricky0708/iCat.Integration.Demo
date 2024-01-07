using Demo.Repositories.Interfaces;
using Demo.Repositories.Models;
using iCat.DB.Client.Factory.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Repositories.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly IDBClientFactory _dbClientFactory;

        public UserRepository(IDBClientFactory dbClientFactory)
        {
            _dbClientFactory = dbClientFactory ?? throw new ArgumentNullException(nameof(dbClientFactory));
        }

        public UserDao? GetUserById(int userId)
        {
            var connection = _dbClientFactory.GetConnection("MainDB");
            var result = default(UserDao);

            // Sample - reader
            //result = connection.ExecuteReader<UserDao>(
            //    "SELECT * FROM User WHERE UserId = @UserId",
            //    new[] { new SqlParameter("@UserId", userId) },
            //    dr => new UserDao
            //    {
            //        // assign user
            //    })?.ToList().FirstOrDefault();

            if (userId == 1)
            {
                result = new UserDao { UserId = 1, UserName = "TestA" };
            }
            else if (userId == 2)
            {
                result = new UserDao { UserId = 1, UserName = "TestA" };
            }

            return result;
        }

        public IEnumerable<UserPermissionDao>? GetPermissionsById(int userId)
        {
            var connection = _dbClientFactory.GetConnection("MainDB");
            var result = default(List<UserPermissionDao>);

            // Sample - reader
            //result = connection.ExecuteReader<UserDao>(
            //    "SELECT * FROM User WHERE UserId = @UserId",
            //    new[] { new SqlParameter("@UserId", userId) },
            //    dr => new UserDao
            //    {
            //        // assign user
            //    })?.ToList().FirstOrDefault();

            if (userId == 1)
            {
                result = new List<UserPermissionDao> {
                    new UserPermissionDao { FunctionValue = 1, Permission = 1 },
                    new UserPermissionDao { FunctionValue = 1, Permission = 4 },
                    new UserPermissionDao { FunctionValue = 2, Permission = 2 },
                    new UserPermissionDao { FunctionValue = 2, Permission = 8 },
                };
            }
            else if (userId == 2)
            {
                result = new List<UserPermissionDao> {
                    new UserPermissionDao { FunctionValue = 2, Permission = 1 },
                    new UserPermissionDao { FunctionValue = 2, Permission = 4 },
                    new UserPermissionDao { FunctionValue = 1, Permission = 2 },
                    new UserPermissionDao { FunctionValue = 1, Permission = 8 },
                };
            }

            return result;
        }
    }
}
