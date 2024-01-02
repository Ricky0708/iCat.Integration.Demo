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
            var connection = _dbClientFactory.GetConnection("key");
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
                result = new UserDao { UserId = 1, UserName = "TestA", Permissions = Shared.enums.Permission.Read | Shared.enums.Permission.Add };
            }
            else if (userId == 2)
            {
                result = new UserDao { UserId = 1, UserName = "TestA", Permissions = Shared.enums.Permission.Read | Shared.enums.Permission.Update };
            }

            return result;
        }
    }
}
