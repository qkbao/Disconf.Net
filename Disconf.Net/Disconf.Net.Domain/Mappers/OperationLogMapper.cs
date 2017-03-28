using DapperExtensions.Mapper;
using Disconf.Net.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Domain.Mappers
{
    public sealed class OperationLogMapper : ClassMapper<OperationLog>
    {
        public OperationLogMapper()
        {
            Table("operation_log");
            Map(f => f.Name).Ignore();
            AutoMap();
        }
    }
}
