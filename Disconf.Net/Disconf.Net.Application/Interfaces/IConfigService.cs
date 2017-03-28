using Disconf.Net.Domain.Condition;
using Disconf.Net.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Interfaces
{
    public interface IConfigService
    {
        Task<IEnumerable<Configs>> GetList(ConfigCondition condition);
        Task<IEnumerable<Configs>> GetList();
        Task<bool> Insert(Configs model);
        Task<bool> Update(Configs model);
        Task<Configs> Get(long id);
        Task<bool> Delete(long id);
        Task<IEnumerable<Configs>> GetConfigsByTemplateIds(string ids);
    }
}
