using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Domain.Models
{
    public abstract class BaseEntity
    {
        public long Id { get; set; }
        public DateTime Ctime { get; set; } = DateTime.Now;
        public DateTime Mtime { get; set; } = DateTime.Now;
    }
}
