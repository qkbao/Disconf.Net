using Disconf.Net.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZooKeeperNet;

namespace Disconf.Net.Core.Zookeeper
{
    /// <summary>
    /// 这个类主要负责监控zookeeper的连接
    /// </summary>
    public class ConnectWatcher : IWatcher, IDisposable
    {
        private string _connectionString;
        private int _sessionTimeOut;
        /// <summary>
        /// 因为程序不可能无限等待，所以需要设置等待zookeeper连接超时的时间，单位毫秒
        /// 至于zookeeper客户端自身实际似乎是会进行无限制连接尝试的
        /// </summary>
        private int _connectTimeOut = 3000;
        /// <summary>
        /// 重连时的时间间隔，单位毫秒
        /// </summary>
        private int _retryIntervalMillisecond = 3000;
        CountDownLatch latch;

        /// <summary>
        /// 该Watcher监控的zookeeper客户端
        /// </summary>
        public ZooKeeper ZooKeeper { get; private set; }
        /// <summary>
        /// 用于监控连接的Zookeeper Watcher
        /// </summary>
        /// <param name="connectionString">zookeeper连接字符串</param>
        /// <param name="timeOut">zookeeper session timout,单位毫秒(ms)</param>
        public ConnectWatcher(string connectionString, int timeOut)
        {
            this._connectionString = connectionString;
            this._sessionTimeOut = timeOut;
            this.Connect();
        }
        public virtual void Process(WatchedEvent @event)
        {
            switch (@event.State)
            {
                case KeeperState.SyncConnected:
                    //测试下来连接和断开连接无论你是否重新注册，都一定会进行通知，而且不论对多少个节点注册了watch，都只会触发一次
                    latch.CountDown();
                    break;
                case KeeperState.Expired:
                    ReConnect();
                    break;
                default:
                    break;
            }
        }
        private void Connect()
        {
            latch = new CountDownLatch(1);
            this.ZooKeeper = new ZooKeeper(this._connectionString, TimeSpan.FromMilliseconds(this._sessionTimeOut), this);
            try
            {
                latch.Await(this._connectTimeOut);
            }
            catch (Exception ex)
            {
                //Connect TimeOut
            }
        }
        static object lockObj = new object();
        internal void ReConnect()
        {
            lock (lockObj)
            {
                while (true)
                {
                    try
                    {
                        if (this.ZooKeeper != null
                            && !ZooKeeper.States.CLOSED.Equals(this.ZooKeeper.State))
                        {
                            break;
                        }
                        this.Dispose();
                        this.Connect();
                        this.ReConnectCallBack();
                    }
                    catch
                    {
                        Thread.Sleep(_retryIntervalMillisecond);
                    }
                }
            }
        }

        protected virtual void ReConnectCallBack()
        {
        }

        public void Dispose()
        {
            if (this.ZooKeeper != null)
            {
                this.ZooKeeper.Dispose();
            }
        }
    }
}
