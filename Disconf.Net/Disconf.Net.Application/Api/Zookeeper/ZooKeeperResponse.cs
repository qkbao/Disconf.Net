using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Api.Zookeeper
{
    public class ZooKeeperResponse : AbstractResponse
    {
        /// <summary>
        /// ZooKeeperHost
        /// </summary>
        public string Host { get; set; }
    }
}
