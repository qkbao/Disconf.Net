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
                    this.ZooKeeper.SetData(zkPath, data, -1);
                }
                else
                {
                    this.ZooKeeper.Create(zkPath, data, Ids.OPEN_ACL_UNSAFE, CreateMode.Persistent);
                }
            });
        }
        public void Remove(string zkPath)
        {
            this._queue.Enqueue(() =>
            {
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
