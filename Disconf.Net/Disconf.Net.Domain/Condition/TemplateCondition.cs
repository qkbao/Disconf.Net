using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Domain.Condition
{
    public class TemplateCondition
    {
        public long? AppId { get; set; }
        public long? EnvId { get; set; }
        public int? Type { get; set; }
        public string Version { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
