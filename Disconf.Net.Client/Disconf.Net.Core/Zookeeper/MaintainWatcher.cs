using Disconf.Net.Core.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace Disconf.Net.Core.Zookeeper
{
    /// <summary>
    /// 负责zk节点设置的监控
    /// </summary>
    public class MaintainWatcher : ConnectWatcher
    {
        readonly ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>();
        /// <summary>
        /// 重试时间间隔
        /// </summary>
        const int RetryIntervalMillisecond = 1000;
        /// <summary>
        /// znode设置类
        /// </summary>
        /// <param name="connectionString">zookeeper连接字符串</param>
        /// <param name="timeOut">zookeeper session timout,单位毫秒(ms)</param>
        public MaintainWatcher(string connectionString, int timeOut)
            : base(connectionString, timeOut)
        {
            Task.Run(() => Execute());
        }
        public void AddOrSetData(string zkPath, byte[] data)
        {
            this._queue.Enqueue(() =>
            {
                var stat = this.ZooKeeper.Exists(zkPath, false);
                if (stat != null)
                {
                    this.RemoveTmpChildNode(zkPath);//先删除子节点，再更新值保证不会出现客户端已经更新完并新增了节点，而服务端还没删完的情况
                    this.ZooKeeper.SetData(zkPath, data, -1);
                }
                else
                {
                    this.ZooKeeper.CreateWithPath(zkPath, data, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
                }
            });
        }
        /// <summary>
        /// 移除临时子节点，表明该节点目前已更新，客户端需要重新下载
        /// </summary>
        /// <param name="path"></param>
        private void RemoveTmpChildNode(string path)
        {
            var childs = this.ZooKeeper.GetChildren(path, false);
            if (childs != null && childs.Any())
            {
                foreach (var child in childs)
                {
                    this.ZooKeeper.Delete(string.Format("{0}/{1}", path, child), -1);
                }
            }
        }
        public void Remove(string zkPath)
        {
            this._queue.Enqueue(() =>
            {
                this.RemoveTmpChildNode(zkPath);//zookeeper在存在子节点时，不允许直接删除父节点，所以需要先删除子节点
                this.ZooKeeper.Delete(zkPath, -1);
            });
        }
        private void Execute()
        {
            while (true)
            {
                Action act;
                if (this._queue.TryPeek(out act)
                    && this.Execute(act))
                {
                    while (!this._queue.TryDequeue(out act))
                    {
                        //do nothing
                    }
                    continue;
                }
                Thread.Sleep(RetryIntervalMillisecond);
            }
        }
        private bool Execute(Action action)
        {
            try
            {
                action();
            }
            catch (KeeperException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                //TODO: 非zk错误，记录日志
            }
            return true;
        }
    }
}
