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
    public sealed class UserMapper : ClassMapper<User>
    {
        public UserMapper()
        {
            Table("user");
            Map(f => f.IsDelete).Column("is_delete");
            Map(f => f.IsSystem).Column("is_system");
            Map(f => f.RoleId).Column("role_id");
            AutoMap();
        }
    }
}
