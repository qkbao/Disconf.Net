using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace Disconf.Net.Core.Zookeeper
{
    /// <summary>
    /// 这个类只负责监控znode的变化
    /// </summary>
    public class NodeWatcher : ConnectWatcher
    {
        #region fileds
        IZkTreeBuilder _builder;
        /// <summary>
        /// 最后一次Node更新的时间戳，用于Expired重连之后，判断哪些znode在Expired这段时间内有过变化
        /// </summary>
        long _mtime;
        /// <summary>
        /// znode发生变化时的回调事件，arg1对应configName
        /// </summary>
        public event Action<string> NodeChanged;
        #endregion

        public NodeWatcher(string connectionString, int timeOut, IZkTreeBuilder builder)
            : base(connectionString, timeOut)
        {
            this._builder = builder;
            this.RegisterWatcher();
        }
        protected override void ReConnectCallBack()
        {
            var configs = this.RegisterWatcher();
            if (this.NodeChanged != null)
            {
                //Expired之后变更的节点需要补调通知
                foreach (var config in configs)
                {
                    if (!string.IsNullOrWhiteSpace(config))
                    {
                        this.NodeChanged(config);
                    }
                }
            }
        }
        /// <summary>
        /// 注册监控关系，并返回在注册监控之前变更过的znode完整路径对应的configName
        /// </summary>
        public IEnumerable<string> RegisterWatcher()
        {
            var configs = new HashSet<string>();
            if (this._builder != null)
            {
                long mtime = this._mtime;
                foreach (var node in this._builder.GetAllZnodes())
                {
                    var path = this._builder.GetZkPath(node);
                    try
                    {
                        var stat = this.ZooKeeper.Exists(path, true);
                        if (stat != null)
                        {
                            if (this._mtime > 0 && stat.Mtime > this._mtime)
                            {//通过_mtime是否大于0进行判断是第一次还是Expired后重连，只有重连时才需要返回变更过的节点
                                configs.Add(this._builder.GetConfigName(node));
                            }
                            mtime = Math.Max(mtime, stat.Mtime);
                        }
                    }
                    catch (Exception ex)
                    {
                        //TODO:可能需要判断Expired
                    }
                }
                this._mtime = mtime;
            }
            return configs;
        }
        public override void Process(WatchedEvent @event)
        {
            base.Process(@event);
            switch (@event.Type)
            {
                case EventType.NodeDataChanged:
                    var path = @event.Path;
                    if (this.NodeChanged != null && !string.IsNullOrWhiteSpace(path))
                    {
                        this.NodeChanged(this._builder.GetConfigName(Path.GetFileName(path)));
                        try
                        {
                            //重新注册监控
                            var stat = this.ZooKeeper.Exists(path, true);
                            this._mtime = stat.Mtime;//按正常逻辑，最后更新的节点，mtime肯定比目前记录的mtime大，所以这里不进行Math.Max处理
                        }
                        catch (Exception ex)
                        {
                            //TODO:可能需要判断Expired
                        }
                    }
                    break;
            }
        }
    }
}
