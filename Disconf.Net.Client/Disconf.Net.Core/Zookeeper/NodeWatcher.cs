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
        private IZkTreeBuilder _builder;
        /// <summary>
        /// 最后一次Node更新的时间戳，用于Expired重连之后，判断哪些znode在Expired这段时间内有过变化
        /// </summary>
        private long _mtime;
        private byte[] _clientName;
        /// <summary>
        /// znode发生变化时的回调事件，arg1对应configName
        /// </summary>
        public event Action<string> NodeChanged;
        #endregion

        /// <summary>
        /// znode监控类
        /// </summary>
        /// <param name="connectionString">zookeeper连接字符串</param>
        /// <param name="timeOut">zookeeper session timout,单位毫秒(ms)</param>
        /// <param name="builder"></param>
        /// <param name="clientName">客户端名称，用于标识更新状况，传入空或null则使用Environment.MachineName</param>
        public NodeWatcher(string connectionString, int timeOut, IZkTreeBuilder builder, string clientName = null)
            : base(connectionString, timeOut)
        {
            this._builder = builder;
            if (string.IsNullOrWhiteSpace(clientName))
            {
                clientName = Environment.MachineName;
            }
            this._clientName = Encoding.UTF8.GetBytes(clientName);
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
                            this.AddTmpChildNode(path);
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
        private void AddTmpChildNode(string path)
        {
            //添加监控时同时增加临时节点，表明客户端已经下载过节点数据，删除节点部分工作由服务端进行
            string nodePath = string.Format("{0}/{1}", path, Guid.NewGuid());
            this.ZooKeeper.Create(nodePath, this._clientName, Ids.OPEN_ACL_UNSAFE, CreateMode.Ephemeral);//注意使用的是临时节点
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
                            //这里可能会存在Expired问题
                            var stat = this.ZooKeeper.Exists(path, true);
                            this.AddTmpChildNode(path);
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
