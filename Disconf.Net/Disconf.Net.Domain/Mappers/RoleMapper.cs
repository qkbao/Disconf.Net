using DapperExtensions.Mapper;
using Disconf.Net.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Domain.Mappers
{
    [Serializable]
    public sealed class RoleMapper : ClassMapper<Role>
    {
        public RoleMapper()
        {
            Table("role");
            Map(f => f.PermissionIds).Column("permission_ids");
            Map(f => f.IsDelete).Column("is_delete");
            Map(f => f.CreateId).Column("create_id");
            AutoMap();
        }
    }
}
