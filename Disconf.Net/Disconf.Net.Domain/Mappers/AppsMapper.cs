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
    public sealed class AppsMapper : ClassMapper<Apps>
    {
        public AppsMapper()
        {
            Table("apps");
            AutoMap();
        }
    }
}
