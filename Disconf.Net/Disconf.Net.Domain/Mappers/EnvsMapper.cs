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
    public sealed class EnvsMapper : ClassMapper<Envs>
    {
        public EnvsMapper()
        {
            Table("envs");
            AutoMap();
        }
    }
}
