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
    public sealed class PermissionMapper : ClassMapper<Permission>
    {
        public PermissionMapper()
        {
            Table("permission");
            Map(f => f.PermissionType).Column("permission_type");
            Map(f => f.IsDelete).Column("is_delete");
            Map(f => f.ParentId).Column("parent_id");
            Map(f => f.AppId).Column("app_id");
            AutoMap();
        }
    }
}
