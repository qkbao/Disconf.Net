using Disconf.Net.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Core.Zookeeper
{
    public abstract class ZkConfigTreeBuilder : ZkTreeBuilder
    {
        public ZkConfigTreeBuilder(string appName, string version, string environment)
            : base(appName, version, environment)
        {
        }
        /// <summary>
        /// 当前对应配置类型
        /// </summary>
        public abstract ConfigType ConfigType { get; }

        public override string GetZkRootPath()
        {
            return Path.Combine(base.GetZkRootPath(), this.ConfigType.ToString());
        }
    }
}
