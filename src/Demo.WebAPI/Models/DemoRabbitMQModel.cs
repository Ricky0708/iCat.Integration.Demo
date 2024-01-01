using iCat.RabbitMQ.Attributes;
using iCat.RabbitMQ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.WebAPI.Models
{
    [Exchange("DemoExchange")]
    public class DemoRabbitMQModel : BaseMQDataModel
    {
        public string Company { get; set; }
        public string Address { get; set; }
    }
}
