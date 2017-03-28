using Disconf.Net.Domain.Condition;
using Disconf.Net.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetList();
        Task<bool> Insert(Role model);
        Task<bool> Update(Role model);
        Task<Role> Get(long id);
        Task<bool> Delete(long id);
        Task<IEnumerable<Role>> GetList(RoleCondition condition);
    }
}
