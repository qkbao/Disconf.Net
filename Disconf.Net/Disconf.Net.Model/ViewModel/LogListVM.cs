using Disconf.Net.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Disconf.Net.Model.ViewModel
{
    public class LogListVM
    {
        public IPagedList<OperationLog> Logs { get; set; }
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 操作时间 End
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 关键词（也就是内容模糊查询）
        /// </summary>
        public string Content { get; set; }
    }
}
