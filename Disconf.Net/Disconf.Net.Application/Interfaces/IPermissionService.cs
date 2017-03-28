using Disconf.Net.Domain.Enum;
using Disconf.Net.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Interfaces
{
    public interface IPermissionService
    {
        Task<IEnumerable<Permission>> GetList();
        Task<bool> Insert(Permission model);
        Task<bool> Update(Permission model);
        Task<Permission> Get(long id);
        Task<bool> Delete(long id);
        Task<bool> BatchInsert(string name);
        Task<bool> BatchInsertByAfterEnv(string name);
        Task<IEnumerable<Permission>> GetPers(string name);
        Task<bool> BatchUpdatePer(string oldName, string newName, PermissionType type);
        Task<bool> BatchDeleteByAppId(long appId);
        Task<bool> BatchDeleteByName(string name);
        Task<bool> AuthorityCheck(string code, long roleId);
    }
}
