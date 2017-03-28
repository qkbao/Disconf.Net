using Disconf.Net.Client.Fetch;
using Disconf.Net.Client.Rules;
using Disconf.Net.Core.Zookeeper;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Disconf.Net.Client
{
    /// <summary>
    /// 配置管理类
    /// </summary>
    public class ConfigManager
    {
        public readonly RuleCollection<FileRule> FileRules = new RuleCollection<FileRule>();
        public readonly RuleCollection<ItemRule> ItemRules = new RuleCollection<ItemRule>();

        public static readonly ConfigManager Instance = new ConfigManager();

        private ClientConfigSection config = ClientConfigSection.Current;
        private NodeWatcher _fileWatcher;
        private NodeWatcher _itemWatcher;

        private ConfigManager()
        {
        }
        
        /// <summary>
        /// Config初始化，包括zookeeper、scan等
        /// </summary>
        /// <param name="config"></param>
        public void Init()
        {
            if (config.EnableRemote)
            {
                var task = Task.Run(() =>
                {
                    this.GetAllConfigs();
                });
                if (config.UpdateStrategy.FirstSync)
                {
                    task.Wait();
                }
            }
        }

        private void GetAllConfigs()
        {
            IEnumerable<string> files;
            IDictionary<string, string> items;
            FetchManager.Instance.GetAllConfigs(out files, out items);
            string zkHosts = FetchManager.Instance.GetZookeeperHosts();
            if (files != null)
            {
                IZkTreeBuilder fileBuilder = new ZkFileTreeBuilder(config.ClientInfo.AppName, config.ClientInfo.Version, config.ClientInfo.Environment);
                foreach (var file in files)
                {
                    fileBuilder.GetOrAddZnodeName(file);
                    this.FileRules.For(file);
                    this.FileRules.ConfigChanged(file, null);
                }
                this._fileWatcher = new NodeWatcher(zkHosts, 30000, fileBuilder);
                this._fileWatcher.NodeChanged += _fileWatcher_NodeChanged;
            }
            if (items != null)
            {
                IZkTreeBuilder itemBuilder = new ZkItemTreeBuilder(config.ClientInfo.AppName, config.ClientInfo.Version, config.ClientInfo.Environment);
                foreach (var item in items.Keys)
                {
                    itemBuilder.GetOrAddZnodeName(item);
                    this.ItemRules.ConfigChanged(item, items[item]);
                }
                this._itemWatcher = new NodeWatcher(zkHosts, 30000, itemBuilder);
                this._itemWatcher.NodeChanged += _itemWatcher_NodeChanged; ;
            }
        }

        private void _itemWatcher_NodeChanged(string obj)
        {
            string value = FetchManager.Instance.GetItem(obj);
            this.ItemRules.ConfigChanged(obj, value);
        }

        private void _fileWatcher_NodeChanged(string obj)
        {
            //更新本地文件
            if (FetchManager.Instance.DownloadFile(obj))
            {
                this.FileRules.ConfigChanged(obj, null);
            }
        }
    }
}
