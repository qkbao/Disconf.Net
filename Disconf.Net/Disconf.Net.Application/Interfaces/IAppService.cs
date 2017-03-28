using Disconf.Net.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Interfaces
{
    public interface IAppService
    {
        Task<IEnumerable<Apps>> GetList();
        Task<bool> Insert(Apps model);
        Task<bool> Update(Apps model);
        Task<Apps> Get(long id);
        Task<bool> Delete(long id);
        Task<long> GetAppIdByName(string name);
        Task<string> GetAppNameById(long id);
    }
}
