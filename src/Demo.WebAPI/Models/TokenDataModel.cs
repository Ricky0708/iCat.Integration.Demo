using Demo.Shared.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Demo.WebAPI.Models
{
    public class TokenDataModel
    {
        public int UserId { get; set; }
        public required string UserName { get; set; }
    }
}
