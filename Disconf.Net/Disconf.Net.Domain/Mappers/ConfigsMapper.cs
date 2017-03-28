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
    public sealed class ConfigsMapper : ClassMapper<Configs>
    {
        public ConfigsMapper()
        {
            Table("configs");
            Map(f => f.EnvId).Column("env_id");
            Map(f => f.TemplateId).Column("template_id");
            Map(f => f.Name).Ignore();
            Map(f => f.Type).Ignore();
            AutoMap();
        }
    }
}
