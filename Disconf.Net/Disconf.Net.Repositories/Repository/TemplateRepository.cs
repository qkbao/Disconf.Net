using Dapper;
using Disconf.Net.Domain.Condition;
using Disconf.Net.Domain.Models;
using Disconf.Net.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Repositories.Repository
{
    public class TemplateRepository : BaseRepository, ITemplateRepository
    {
        public async Task<IEnumerable<Templates>> GetTemplateList(TemplateCondition condition)
        {
            string sql = $@"SELECT tem.id, tem.name,tem.description,tem.default_value as defaultvalue,tem.type,tem.version,con.id as ConfigId, con.value as configvalue From templates as tem
                            LEFT JOIN configs as con ON tem.id = con.template_id AND con.env_id = @envId 
                            {GetSqlWhere(condition)}";
            return await ExecuteWithConditionAsync(async s => await s.QueryAsync<Templates>(sql, GetSqlParameter(condition)));
        }
        private string GetSqlWhere(TemplateCondition condition)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" WHERE 1=1 ");
            if (condition.AppId.HasValue)
            {
                sb.Append(" AND tem.app_id = @appId ");
            }
            if (condition.Type.HasValue)
            {
                sb.Append(" AND tem.type = @type ");
            }
            if (!string.IsNullOrWhiteSpace(condition.Version))
            {
                sb.Append(" AND tem.version = @version ");
            }
            if (!string.IsNullOrWhiteSpace(condition.Name))
            {
                sb.Append(" AND tem.name = @name ");
            }
            return sb.ToString();
        }

        private object GetSqlParameter(TemplateCondition condition)
        {
            return new
            {
                appId = condition.AppId,
                envId = condition.EnvId,
                type = condition.Type,
                version = condition.Version,
                name = condition.Name.Trim()
            };
        }
    }
}
