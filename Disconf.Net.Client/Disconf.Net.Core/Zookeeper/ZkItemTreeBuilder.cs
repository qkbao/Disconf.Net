using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disconf.Net.Core.Model;

namespace Disconf.Net.Core.Zookeeper
{
    public class ZkItemTreeBuilder : ZkConfigTreeBuilder
    {
        public ZkItemTreeBuilder(string appName, string version, string environment)
            : base(appName, version, environment)
        {
        }
        public override ConfigType ConfigType
        {
            get
            {
                return ConfigType.Item;
            }
        }
    }
}
