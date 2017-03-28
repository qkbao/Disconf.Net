using Disconf.Net.Client.Fetch;
using Disconf.Net.Client.Rules;
using Disconf.Net.Core.Zookeeper;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        /// <summary>
        /// 更新异常时调用事件进行通知
        /// </summary>
        public event Action<Exception> Faulted;

        private ClientConfigSection config = ClientConfigSection.Current;
        private NodeWatcher _fileWatcher;
        private NodeWatcher _itemWatcher;
        private ExceptionHandler _handler;

        private ConfigManager()
        {
            this._handler = new ExceptionHandler();
            this._handler.Faulted += _handler_Faulted;
        }

        private void _handler_Faulted(string arg1, Exception arg2)
        {
            if (this.Faulted != null)
            {
                if (!string.IsNullOrWhiteSpace(arg1))
                {
                    arg2 = new Exception(arg1, arg2);
                }
                this.Faulted(arg2);
            }
        }

        /// <summary>
        /// Config初始化，包括zookeeper、scan等
        /// 该方法理应在所有代码执行之前就被调用，否则可能会出现配置调用顺序错误
        /// </summary>
        /// <param name="config"></param>
        public void Init()
        {
            if (config.EnableRemote)
            {
                var task = Task.Run(() =>
                {
                    this._handler.Execute(() => this.GetAllConfigs(), string.Empty);
                 });
                if (config.UpdateStrategy.StartedSync)
                {
                    task.Wait();
                }
            }
        }

        private void GetAllConfigs()
        {
            IEnumerable<string> files = null;
            IDictionary<string, string> items = null;
            string zkHosts = null;
            var fetchManager = FetchManager.Instance;
            Exception ex = null;
            bool downLoad = false;
            try
            {
                zkHosts = fetchManager.GetZookeeperHosts();
                var ltimeFromLocal = this.GetLastChangedTimeFromLocalIfExist();
                var ltimeFromServer = fetchManager.GetLastChangedTime();
                if (ltimeFromLocal > DateTime.Now || ltimeFromLocal < ltimeFromServer)
                {
                    fetchManager.GetAllConfigs(out files, out items);
                    downLoad = true;
                    fetchManager.SaveLastChangedTime(ltimeFromServer);
                }
            }
            catch (Exception e)
            {
                ex = e;
            }
            if (!downLoad)
            {
                //如果更新异常、或者不需要从服务端获取，则从本地恢复item
                items = this.GetItemsFromLocalIfExist();
                //file方式虽然本身就是替换了实际文件的，但发布时配置文件存在覆盖问题，所以也需要恢复
                files = this.GetFilesFromLocalIfExist();
                fetchManager.CopyFiles(files);
            }
            this.RefreshAndInitItems(zkHosts, items);
            this.RefreshAndInitFiles(zkHosts, files);
            if (ex != null)
            {
                throw ex;
            }
        }
        private void RefreshAndInitFiles(string zkHosts, IEnumerable<string> files)
        {
            if (files != null)
            {
                IZkTreeBuilder fileBuilder = new ZkFileTreeBuilder(config.ClientInfo.AppName, config.ClientInfo.Version, config.ClientInfo.Environment);
                foreach (var file in files)
                {
                    fileBuilder.GetOrAddZnodeName(file);
                    this._handler.Execute(() =>
                    {
                        if (!config.UpdateStrategy.FileIgnoreList.Contains(file))
                        {
                            //对于文件类型，如果本地没配置过Rule规则，则采用默认规则，即将文件复制到实际位置，然后RefreshSection配置对应的文件名
                            this.FileRules.For(file);
                            this.FileRules.ConfigChanged(file, null);
                        }
                    }, string.Format("Some thing is wrong with file '{0}'", file));//除了外部包含保证Exception不会导致程序异常，方法内部同样需要保证单次异常不会导致其它监控流程失败
                }
                if (!string.IsNullOrWhiteSpace(zkHosts))
                {
                    this._fileWatcher = new NodeWatcher(zkHosts, 30000, fileBuilder, config.ClientInfo.ClientName);
                    this._fileWatcher.NodeChanged += _fileWatcher_NodeChanged;
                }
            }
        }
        private void RefreshAndInitItems(string zkHosts, IDictionary<string, string> items)
        {
            if (items != null)
            {
                IZkTreeBuilder itemBuilder = new ZkItemTreeBuilder(config.ClientInfo.AppName, config.ClientInfo.Version, config.ClientInfo.Environment);
                foreach (var item in items.Keys)
                {
                    itemBuilder.GetOrAddZnodeName(item);
                    this._handler.Execute(() =>
                    {
                        if (!config.UpdateStrategy.ItemIgnoreList.Contains(item))
                        {
                            //键值对必须配置本地规则，否则无法处理
                            this.ItemRules.ConfigChanged(item, items[item]);
                        }
                    }, string.Format("Some thing is wrong with item '{0}'", item));
                }
                if (!string.IsNullOrWhiteSpace(zkHosts))
                {
                    this._itemWatcher = new NodeWatcher(zkHosts, 30000, itemBuilder, config.ClientInfo.ClientName);
                    this._itemWatcher.NodeChanged += _itemWatcher_NodeChanged;
                }
            }
        }
        private IEnumerable<string> GetFilesFromLocalIfExist()
        {
            IEnumerable<string> files = null;
            var fileName = config.Preservation.TmpFilesLocalName;
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                string path = Path.Combine(config.Preservation.TmpRootPhysicalPath, fileName);
                if (File.Exists(path))
                {
                    files = File.ReadAllText(path, Encoding.UTF8).Split(',');
                }
            }
            return files;
        }
        private IDictionary<string, string> GetItemsFromLocalIfExist()
        {
            IDictionary<string, string> dic = null;
            var fileName = config.Preservation.TmpItemsLocalName;
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                string path = Path.Combine(config.Preservation.TmpRootPhysicalPath, fileName);
                try
                {
                    XElement root = XElement.Load(path);
                    var eles = root.Elements("item");
                    if (eles.Any())
                    {
                        dic = eles.ToDictionary(e => e.Attribute("key").Value, e => e.Attribute("value").Value);
                    }
                }
                catch { }
            }
            return dic;
        }
        private DateTime GetLastChangedTimeFromLocalIfExist()
        {
            var fileName = config.Preservation.TmpTimeLocalName;
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                string path = Path.Combine(config.Preservation.TmpRootPhysicalPath, fileName);
                try
                {
                    DateTime dt;
                    if (DateTime.TryParse(File.ReadAllText(path), out dt))
                    {
                        return dt;
                    }
                }
                catch { }
            }
            return DateTime.MinValue;
        }
        private void _itemWatcher_NodeChanged(string obj)
        {
            this._handler.Execute(() =>
            {
                if (!config.UpdateStrategy.ItemIgnoreList.Contains(obj))
                {
                    string value = FetchManager.Instance.GetItem(obj);
                    this.ItemRules.ConfigChanged(obj, value);
                }
            }, string.Format("Some thing is wrong with item '{0}'", obj));
        }
        private void _fileWatcher_NodeChanged(string obj)
        {
            this._handler.Execute(() =>
            {
                if (!config.UpdateStrategy.FileIgnoreList.Contains(obj))
                {
                    //更新本地文件
                    if (FetchManager.Instance.DownloadFile(obj))
                    {
                        this.FileRules.ConfigChanged(obj, null);
                    }
                }
            }, string.Format("Some thing is wrong with file '{0}'", obj));
        }
    }
}
