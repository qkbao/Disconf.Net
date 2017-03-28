using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Core.Model
{
    public class FetchFilter
    {
        /// <summary>
        /// 系统名
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// 系统环境
        /// </summary>
        public string Environment { get; set; }
        /// <summary>
        /// 系统版本
        /// </summary>
        public string Version { get; set; }
    }
    public class ConfigFetchFilter : FetchFilter
    {
        /// <summary>
        /// 配置名
        /// </summary>
        public string ConfigName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public ConfigType ConfigType { get; set; }
    }
}
