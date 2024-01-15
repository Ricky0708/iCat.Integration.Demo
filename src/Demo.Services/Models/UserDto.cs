using Demo.Repositories.Models;
using iCat.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Services.Models
{
    public class UserDto : UserDao
    {
        public IEnumerable<Permit>? Permissions { get; set; }
    }
}
