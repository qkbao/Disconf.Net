using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Concurrent;
using Disconf.Net.Domain.Models;
using System.Threading.Tasks;
using Autofac;
using Disconf.Net.Application.Interfaces;
using Disconf.Net.Domain.Config;

namespace Disconf.Net.Web
{
    public class LogQueue
    {
        #region Ctors

        static LogQueue()
        {
            AppLogQueue = new ConcurrentQueue<OperationLog>();
            appLogTask = Task.Run(() => WriteAppLog());
            logService = ((IContainer)Bootstrap.Bootstrapper.Container).Resolve<ILogService>();
            logConfiguration = Its.Configuration.Settings.Get<LogConfiguration>();
        }

        #endregion Ctors

        #region Fields

        /// <summary>
        /// 应用程序日志任务
        /// </summary>
        private static Task appLogTask;

        private static ILogService logService;

        private static LogConfiguration logConfiguration;

        #endregion Fields

        #region Properties

        /// <summary>
        /// 线程安全的应用程序日志队列
        /// </summary>
        public static ConcurrentQueue<OperationLog> AppLogQueue { get; private set; }

        #endregion Properties

        #region Private Methods

        /// <summary>
        /// 日志分组
        /// </summary>
        /// <param name="logs"></param>
        /// <returns></returns>
        private static List<List<OperationLog>> LogGrouping(List<OperationLog> logs)
        {
            var logGroupCount = logConfiguration.LogGroupCount;
            var group = new List<List<OperationLog>>();
            var count = logs.Count();
            var groupListCount = (int)Math.Ceiling((decimal)count / logGroupCount);
            for (var i = 0; i < groupListCount; i++)
            {
                var groupItem = new List<OperationLog>();
                for (var y = 0; y < logGroupCount; y++)
                {
                    var index = y + logGroupCount * i;
                    if (index < logs.Count)
                    {
                        groupItem.Add(logs[index]);
                    }
                    else
                    {
                        break;
                    }
                }
                group.Add(groupItem);
            }
            return group;
        }

        /// <summary>
        /// 发送日志
        /// </summary>
        /// <param name="logs"></param>
        private async static Task SendLogs(List<OperationLog> logs)
        {
            var logGroup = LogGrouping(logs);
            if (logGroup.Count > 0)
            {
                foreach (var log in logGroup)
                {
                    await logService.InsertBatch(log);
                }
            }
        }

        /// <summary>
        /// 写入应用程序日志
        /// </summary>
        private async static void WriteAppLog()
        {
            while (true)
            {
                var logDeQueueList = new List<OperationLog>();
                var logQueueCount = AppLogQueue.Count;
                if (logQueueCount > 0 && AppLogQueue.Count <= logConfiguration.QueueLimit)
                {
                    for (var i = 0; i < logQueueCount; i++)
                    {
                        var log = new OperationLog();
                        if (AppLogQueue.TryDequeue(out log))
                        {
                            logDeQueueList.Add(log);
                        }
                    }
                }
                if (logDeQueueList.Count > 0)
                {
                    await SendLogs(logDeQueueList);
                }
                System.Threading.Thread.Sleep(logConfiguration.WriteLogInterval);
            }
        }

        #endregion Private Methods
    }
}