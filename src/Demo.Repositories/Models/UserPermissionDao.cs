using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Repositories.Models
{
    public class UserPermissionDao
    {
        public int FunctionValue { get; set; }
        public string FunctionName { get; set; }
        public int Permission { get; set; }
        public string PermissionName { get; set; }
    }
}
