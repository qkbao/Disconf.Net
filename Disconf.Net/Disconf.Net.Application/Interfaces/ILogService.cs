using Disconf.Net.Domain.Models;
using Disconf.Net.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Interfaces
{
    public interface ILogService
    {
        Task<bool> Insert(OperationLog log);

        Task<bool> Update(OperationLog log);

        Task<bool> Delete(long id);

        Task<OperationLog> GetLog(long id);

        Task<IEnumerable<OperationLog>> GetLogList();

        Task<IEnumerable<OperationLog>> GetLogList(int pageSize, LogPagingFilteringVM command);

        Task<int> GetLogTotal(LogPagingFilteringVM command);

        Task InsertBatch(IEnumerable<OperationLog> logs);
    }
}
