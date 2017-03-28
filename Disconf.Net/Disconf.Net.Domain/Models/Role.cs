using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Domain.Models
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }
        public string PermissionIds { get; set; }
        public long CreateId { get; set; }
        public bool IsDelete { get; set; }
    }
}
