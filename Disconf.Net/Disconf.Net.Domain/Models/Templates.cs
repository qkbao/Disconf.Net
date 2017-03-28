using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Domain.Models
{
    public class Templates : BaseEntity
    {
        public long AppId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DefaultValue { get; set; }
        public string ConfigValue { get; set; }
        public int Type { get; set; }
        public string Version { get; set; }
        public bool IsDelete { get; set; }
        public long ConfigId { get; set; }
        public string ZookeeperChildren { get; set; }
    }
}
