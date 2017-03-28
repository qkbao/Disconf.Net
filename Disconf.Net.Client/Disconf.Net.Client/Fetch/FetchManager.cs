using Disconf.Net.Core.Model;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
                        this.SaveAndCopyFile(key, fileDic[key]);
                    }
                    this.SaveFileList(files);
                }
                if (dic.ContainsKey(ConfigType.Item) && dic[ConfigType.Item] != null)
                {
                    items = dic[ConfigType.Item];
                    this.SaveItems(items);
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
            this.SaveAndCopyFile(configName, content);
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
            var value = this._fetcher.GetConfig(filter);
            this.AddOrSetItem(configName, value);
            return value;
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
        private void SaveAndCopyFile(string configName, string content)
        {
            string tmpPath = Path.Combine(config.Preservation.TmpRootPhysicalPath, configName);
            File.WriteAllText(tmpPath, content, Encoding.UTF8);
            this.CopyFile(configName);
        }
        private void CopyFile(string configName)
        {
            string tmpPath = Path.Combine(config.Preservation.TmpRootPhysicalPath, configName);
            string factPath = Path.Combine(config.Preservation.FactRootPhysicalPath, configName);
            File.Copy(tmpPath, factPath, true);
        }
        public void CopyFiles(IEnumerable<string> files)
        {
            if (files != null)
            {
                foreach (var file in files)
                {
                    try
                    {
                        this.CopyFile(file);
                    }
                    catch
                    {
                    }
                }
            }
        }
        public void SaveLastChangedTime(DateTime ltime)
        {
            var fileName = config.Preservation.TmpTimeLocalName;
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                string path = Path.Combine(config.Preservation.TmpRootPhysicalPath, fileName);
                File.WriteAllText(path, ltime.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }
        private void SaveFileList(IEnumerable<string> files)
        {
            StringBuilder tmp = new StringBuilder();
            foreach (var file in files)
            {
                tmp.Append(',');
                tmp.Append(file);
            }
            if (tmp.Length > 0)
            {
                tmp.Remove(0, 1);
                string path = Path.Combine(config.Preservation.TmpRootPhysicalPath, config.Preservation.TmpFilesLocalName);
                File.WriteAllText(path, tmp.ToString(), Encoding.UTF8);
            }
        }
        private void SaveItems(IDictionary<string, string> items)
        {
            var fileName = config.Preservation.TmpItemsLocalName;
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                Task.Run(() =>
                {
                    string path = Path.Combine(config.Preservation.TmpRootPhysicalPath, fileName);
                    XElement root = new XElement("items");
                    foreach (var kv in items)
                    {
                        this.AddElementItem(root, kv.Key, kv.Value);
                    }
                    root.Save(path);
                });
            }
        }
        private void AddElementItem(XElement root, string key, string value)
        {
            root.Add(new XElement("item", new XAttribute("key", key), new XAttribute("value", value)));
        }
        private void AddOrSetItem(string key, string value)
        {
            var fileName = config.Preservation.TmpItemsLocalName;
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                Task.Run(() =>
                {
                    string path = Path.Combine(config.Preservation.TmpRootPhysicalPath, fileName);
                    try
                    {
                        XElement root = XElement.Load(path);
                        var ele = root.Elements("item").FirstOrDefault(e => e.Attribute("key").Value == key);
                        if (ele == null)
                        {
                            this.AddElementItem(root, key, value);
                        }
                        else
                        {
                            ele.Attribute("value").SetValue(value);
                        }
                        root.Save(path);
                    }
                    catch { }
                });
            }
        }
        public DateTime GetLastChangedTime()
        {
            var content = this._fetcher.GetLastChangedTime(new FetchFilter
            {
                AppName = config.ClientInfo.AppName,
                Version = config.ClientInfo.Version,
                Environment = config.ClientInfo.Environment
            });
            return DateTime.Parse(content);
        }
    }
}
