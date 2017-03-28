using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Domain.Models
{
    public class Permission : BaseEntity
    {
        public string Name { get; set; }
        public long ParentId { get; set; }
        public int PermissionType { get; set; }
        public long AppId { get; set; }
        public string Code { get; set; }
        public bool IsDelete { get; set; }
    }
}
