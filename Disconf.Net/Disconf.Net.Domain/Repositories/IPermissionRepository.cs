using Disconf.Net.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Domain.Repositories
{
    public interface IPermissionRepository : IBaseRepository
    {
        Task<bool> BatchDeleteByAppId(long appId);
        Task<bool> BatchDeleteByName(string name);
        Task<IEnumerable<Permission>> GetPermissionList(string idList);
    }
}
