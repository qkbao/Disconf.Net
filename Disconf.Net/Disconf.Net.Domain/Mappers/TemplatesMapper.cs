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
    public sealed class TemplatesMapper : ClassMapper<Templates>
    {
        public TemplatesMapper()
        {
            Table("templates");
            Map(f => f.AppId).Column("app_id");
            Map(f => f.DefaultValue).Column("default_value");
            Map(f => f.IsDelete).Column("is_delete");
            Map(f => f.ConfigValue).Ignore();
            Map(f => f.ConfigId).Ignore();
            Map(f => f.ZookeeperChildren).Ignore();
            AutoMap();
        }
    }
}
