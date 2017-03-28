using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Domain.Models
{
    public class Configs : BaseEntity
    {
        public long TemplateId { get; set; }
        public long EnvId { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
    }
}
