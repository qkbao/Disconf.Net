using Disconf.Net.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Domain.Repositories
{
    public interface ILogRepository : IBaseRepository
    {
        Task InsertBatch(IEnumerable<OperationLog> logs);
    }
}
