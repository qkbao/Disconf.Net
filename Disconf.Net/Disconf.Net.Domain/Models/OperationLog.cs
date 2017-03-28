using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Domain.Models
{
    public class OperationLog
    {
        public long Id { get; set; }
        public long UId { get; set; }
        public string Content { get; set; }
        public string Name { get; set; }
        public DateTime Ctime { get; set; }
    }
}
