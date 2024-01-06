using Demo.Shared.enums;
using iCat.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Repositories.Models
{
    public class UserDao
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
    }
}
