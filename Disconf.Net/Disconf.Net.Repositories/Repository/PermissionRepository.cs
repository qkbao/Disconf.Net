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
    public class PermissionRepository : BaseRepository, IPermissionRepository
    {

        public async Task<bool> BatchDeleteByAppId(long appId)
        {
            string sql = $@"Delete FROM permission WHERE app_id = @appId";
            return await ExecuteWithConditionAsync(async s => await s.ExecuteScalarAsync<bool>(sql, new
            {
                appId = appId
            }));
        }

        public async Task<bool> BatchDeleteByName(string name)
        {
            string sql = $@"Delete FROM permission WHERE name = @name";
            return await ExecuteWithConditionAsync(async s => await s.ExecuteScalarAsync<bool>(sql, new
            {
                name = name
            }));
        }

        /// <summary>
        /// 根据id集合获取权限列表
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Permission>> GetPermissionList(string idList)
        {
            var sql = $"SELECT id, Name, app_id as appid , code ,permission_type as PermissionType, ctime, mtime FROM permission WHERE id in ( {idList} ) and is_delete=0";
            return await ExecuteWithConditionAsync(async s => await s.QueryAsync<Permission>(sql));
        }
    }
}
