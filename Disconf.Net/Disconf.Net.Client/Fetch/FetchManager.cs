using Disconf.Net.Client.Utils;
using Disconf.Net.Core.Model;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Client.Fetch
{
    public class FetchManager
    {
        private readonly RetryPolicy _policy;
        private readonly IFetcher _fetcher;
        private ClientConfigSection config = ClientConfigSection.Current;

        public static readonly FetchManager Instance = new FetchManager();

        private FetchManager()
        {
            this._policy = this.GetFixedRetryPolicy(config.UpdateStrategy.RetryTimes, config.UpdateStrategy.RetryIntervalSeconds);
            this._fetcher = new Fetcher(config.WebApiHost, this._policy);
        }
        public string GetZookeeperHosts()
        {
            return this._fetcher.GetZkHosts();
        }
        /// <summary>
        /// 获取所有配置
        /// </summary>
        /// <param name="files">获取到的所有file配置</param>
        /// <param name="items">所有到的所有item配置</param>
        public void GetAllConfigs(out IEnumerable<string> files, out IDictionary<string, string> items)
        {
            files = new HashSet<string>();
            items = new Dictionary<string, string>();
            var content = this._fetcher.GetAllConfigs(new FetchFilter
            {
                AppName = config.ClientInfo.AppName,
                Version = config.ClientInfo.Version,
                Environment = config.ClientInfo.Environment
            });
            var dic = JsonConvert.DeserializeObject<Dictionary<ConfigType, Dictionary<string, string>>>(content);
            if (dic != null)
            {
                if (dic.ContainsKey(ConfigType.File) && dic[ConfigType.File] != null)
                {
                    var fileDic = dic[ConfigType.File];
                    files = fileDic.Keys;
                    foreach (var key in fileDic.Keys)
                    {
                        this.SaveFile(key, fileDic[key]);
                    }
                }
                if (dic.ContainsKey(ConfigType.File) && dic[ConfigType.Item] != null)
                {
                    items = dic[ConfigType.Item];
                }
            }
        }
        /// <summary>
        /// 根据配置名下载文件
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public bool DownloadFile(string configName)
        {
            var filter = this.GetFilter(configName, ConfigType.File);
            string content = this._fetcher.GetConfig(filter);
            this.SaveFile(configName, content);
            return true;
        }
        /// <summary>
        /// 根据配置名获取对应的值
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public string GetItem(string configName)
        {
            var filter = this.GetFilter(configName, ConfigType.Item);
            return this._fetcher.GetConfig(filter);
        }
        private RetryPolicy GetFixedRetryPolicy(int retryCount, int retryIntervalSeconds)
        {
            FixedInterval interval = new FixedInterval(retryCount, TimeSpan.FromSeconds(retryIntervalSeconds));
            return new RetryPolicy(RetryPolicy.DefaultFixed.ErrorDetectionStrategy, interval);
        }
        private ConfigFetchFilter GetFilter(string configName, ConfigType type)
        {
            return new ConfigFetchFilter
            {
                AppName = config.ClientInfo.AppName,
                Version = config.ClientInfo.Version,
                Environment = config.ClientInfo.Environment,
                ConfigType = type,
                ConfigName = configName
            };
        }
        private void SaveFile(string configName, string content)
        {
            string tmpPath = Path.Combine(config.Directory.TmpRootPhysicalPath, configName);
            string factPath = Path.Combine(config.Directory.FactRootPhysicalPath, configName);
            File.WriteAllText(tmpPath, content, Encoding.UTF8);
            File.Copy(tmpPath, factPath, true);
        }
    }
}
