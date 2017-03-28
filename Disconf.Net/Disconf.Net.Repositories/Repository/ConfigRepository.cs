using Dapper;
using Disconf.Net.Domain.Models;
using Disconf.Net.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Repositories.Repository
{
    public class ConfigRepository : BaseRepository, IConfigRepository
    {
        public async Task<DateTime> GetConfigLastTime(string appId, string version, string envId)
        {
            string sql = $@"SELECT MAX(c.mtime) FROM configs AS c  
                            LEFT JOIN templates AS t on t.app_id = {appId} and t.version = '{version}' 
                            WHERE c.env_id = {envId}";
            return await ExecuteWithConditionAsync(async s => await s.ExecuteScalarAsync<DateTime>(sql));
        }

        public async Task<IEnumerable<Configs>> GetConfigs(string appId, string version, string envId, int? type, string configName)
        {
            string sql = $@"SELECT t.name,c.value,t.type FROM configs AS c  
                            LEFT JOIN templates AS t ON t.app_id = {appId} AND t.id = c.template_id   
                            WHERE c.env_id = {envId} AND t.version = '{version}' {GetSqlWhere(type, configName)}";
            return await ExecuteWithConditionAsync(async s => await s.QueryAsync<Configs>(sql, GetSqlParameter(type, configName)));
        }

        public async Task<IEnumerable<Configs>> GetConfigsByTemplateIds(string ids)
        {
            string sql = $@"SELECT id, template_id as templateid,env_id as envid ,value FROM configs WHERE template_id in ( {ids} )";
            return await ExecuteWithConditionAsync(async s => await s.QueryAsync<Configs>(sql));
        }
        private string GetSqlWhere(int? type, string configName)
        {
            StringBuilder sb = new StringBuilder();
            if (type.HasValue)
            {
                sb.Append(" AND t.type = @type ");
            }
            if (!string.IsNullOrWhiteSpace(configName))
            {
                sb.Append(" AND t.name = @configName ");
            }
            return sb.ToString();
        }

        private object GetSqlParameter(int? type, string configName)
        {
            return new
            {
                type = type,
                configName = configName
            };
        }
    }
}
