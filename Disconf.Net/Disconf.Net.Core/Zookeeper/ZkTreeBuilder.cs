using Disconf.Net.Core.Model;
using Disconf.Net.Core.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Core.Zookeeper
{
    /// <summary>
    /// zk tree构造类，该类构造的树除去共有的主节点部分外，所以节点均为同级节点
    /// </summary>
    public class ZkTreeBuilder : IZkTreeBuilder
    {
        /// <summary>
        /// 用于存储znodeName与configName对应关系的字典,Key为znodeName,Value为configName
        /// </summary>
        protected ConcurrentDictionary<string, string> _dic = new ConcurrentDictionary<string, string>();
        /// <summary>
        /// 应用名
        /// </summary>
        public string AppName { get; private set; }
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; private set; }
        /// <summary>
        /// 环境
        /// </summary>
        public string Environment { get; private set; }
        public ZkTreeBuilder(string appName, string version, string environment)
        {
            this.AppName = appName;
            this.Version = version;
            this.Environment = environment;
        }

        public virtual string GetOrAddZnodeName(string configName)
        {
            //这里忽略SHA1理论上也存在重复的可能性
            string znodeName = HashAlgorithmHelper<SHA1CryptoServiceProvider>.ComputeHash(configName);
            this._dic[znodeName] = configName;
            return znodeName;
        }
        public string GetConfigName(string znodeName)
        {
            string configName;
            this._dic.TryGetValue(znodeName, out configName);
            return configName;
        }
        public virtual string GetZkRootPath()
        {
            return Path.Combine("\\", AppName, HashAlgorithmHelper<MD5CryptoServiceProvider>.ComputeHash(Version), Environment);
        }
        public virtual string GetZkPath(string znodeName)
        {
            return Path.Combine(this.GetZkRootPath(), znodeName);
        }

        public IEnumerable<string> GetAllZnodes()
        {
            return this._dic.Keys;
        }
    }
}
