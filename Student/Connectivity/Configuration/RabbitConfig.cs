using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectivity.Configuration
{
    public class RabbitConfig
    {
        public static readonly string RabbitMqConnectionHost = "localhost";
        public static readonly string RabbitMqConnectionVirtualHost = "/";
        public static readonly string RabbitMqUsername = "guest";
        public static readonly string RabbitMqPassword= "guest";
    }
}
