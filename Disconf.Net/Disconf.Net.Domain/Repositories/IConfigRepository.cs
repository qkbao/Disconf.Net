using Disconf.Net.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Domain.Repositories
{
    public interface IConfigRepository : IBaseRepository
    {
        Task<DateTime> GetConfigLastTime(string appId, string version, string envId);
        Task<IEnumerable<Configs>> GetConfigs(string appId, string version, string envId, int? type,string configName);
        Task<IEnumerable<Configs>> GetConfigsByTemplateIds(string ids);
    }
}
