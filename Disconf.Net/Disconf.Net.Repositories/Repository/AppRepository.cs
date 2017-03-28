using Dapper;
using Disconf.Net.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Repositories.Repository
{
    public class AppRepository : BaseRepository, IAppRepository
    {
        public async Task<string> GetAppIdByName(string name)
        {
            string sql = $@"SELECT id FROM apps WHERE name = '{name.Trim()}'";
            return await ExecuteWithConditionAsync(async s => await s.ExecuteScalarAsync<string>(sql));
        }

    }
}
