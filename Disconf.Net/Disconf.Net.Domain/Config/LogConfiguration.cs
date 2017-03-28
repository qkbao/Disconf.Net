using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Domain.Config
{
    public class LogConfiguration
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 日志分组发送数
        /// </summary>
        public int LogGroupCount { get; set; } = 10;

        /// <summary>
        /// 队列上限
        /// </summary>
        public int QueueLimit { get; set; } = 1000;

        /// <summary>
        /// 写日志间隔
        /// </summary>
        public int WriteLogInterval { get; set; } = 5000;
    }
}
