using Disconf.Net.Domain.Condition;
using Disconf.Net.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Interfaces
{
    public interface IEnvService
    {
        Task<IEnumerable<Envs>> GetList();
        Task<bool> Insert(Envs model);
        Task<bool> Update(Envs model);
        Task<Envs> Get(long id);
        Task<bool> Delete(long id);
        Task<IEnumerable<Envs>> GetList(EnvCondition condition);
        Task<long> GetEnvIdByName(string name);
        Task<string> GetEnvNameById(long id);
    }
}
