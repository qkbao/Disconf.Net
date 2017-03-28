using Disconf.Net.Domain.Models;
using Disconf.Net.Domain.Repositories;
using Disconf.Net.Infrastructure.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Repositories.Repository
{
    public class LogRepository : BaseRepository, ILogRepository
    {

        public async Task InsertBatch(IEnumerable<OperationLog> logs)
        {
            foreach (var q in logs)
            {
                await this.Inserts(q);
            }
        }

        private async Task Inserts(OperationLog log)
        {
            StringBuilder tmp = new StringBuilder();
            var i = 0;
            List<MySqlParameter> paramters = new List<MySqlParameter>();

            tmp.Append(",(");
            tmp.Append("?uid");
            tmp.Append(i);
            tmp.Append(",?content");
            tmp.Append(i);
            tmp.Append(",?ctime");
            tmp.Append(i);
            tmp.Append(')');
            paramters.Add(new MySqlParameter(string.Format("?uid{0}", i), MySqlDbType.Int64) { Value = log.UId });
            paramters.Add(new MySqlParameter(string.Format("?content{0}", i), MySqlDbType.VarString) { Value = log.Content });
            paramters.Add(new MySqlParameter(string.Format("?ctime{0}", i), MySqlDbType.Timestamp) { Value = log.Ctime });
            i++;
            tmp.Remove(0, 1);
            tmp.Insert(0, $"insert into operation_log (uid,content,ctime) values ");

            using (var connection = Database.Connection(DataBaseConnection.LOCAL_CONNECTION))
            {
                await connection.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(tmp.ToString(), (MySqlConnection)connection);
                cmd.Parameters.AddRange(paramters.ToArray());
                await cmd.ExecuteNonQueryAsync();
            }

        }
    }
}
