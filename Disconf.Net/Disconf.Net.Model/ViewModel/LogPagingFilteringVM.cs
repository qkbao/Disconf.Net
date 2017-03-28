using Disconf.Net.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Model.ViewModel
{
    public class LogPagingFilteringVM
    {
        public int Page { get; set; } = 1;

        /// <summary>
        /// 操作时间 Start
        /// </summary>
        public DateTime? StartTime { get; set; } = DateTime.Now.FormatSearchStartTime();

        /// <summary>
        /// 操作时间 End
        /// </summary>
        public DateTime? EndTime { get; set; } = DateTime.Now.FormatSearchEndTime();
        /// <summary>
        /// 关键词（也就是内容模糊查询）
        /// </summary>
        public string Content { get; set; } = string.Empty;
    }
}
