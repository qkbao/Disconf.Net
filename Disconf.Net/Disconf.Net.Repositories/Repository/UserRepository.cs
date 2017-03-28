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
    public class UserRepository : BaseRepository, IUserRepository
    {
        /// <summary>
        /// 根据roleid集合获取权限列表
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetUserList(string roleidList)
        {
            var sql = $"SELECT id, name, username , password ,role_id as roleid, ctime, mtime FROM user WHERE role_id in ( {roleidList} ) and is_delete = 0";
            return await ExecuteWithConditionAsync(async s => await s.QueryAsync<User>(sql));
        }
    }
}
