using Dapper;
using DapperExtensions;
using DapperExtensions.Mapper;
using DapperExtensions.Sql;
using Disconf.Net.Domain.Repositories;
using Disconf.Net.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Repositories
{
    public abstract partial class BaseRepository : IBaseRepository
    {
        #region Fields

        protected Lazy<DapperExtensionsConfiguration> Configuration = new Lazy<DapperExtensionsConfiguration>(() => new DapperExtensionsConfiguration(typeof(AutoClassMapper<>), new List<Assembly>(), new MySqlDialect()));

        #endregion Fields

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual async Task<bool> Insert<T>(T t) where T : class
        {
            return await ExecuteWithCondition<bool>(c => c.Insert(t) > 0);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual async Task<bool> Update<T>(T t) where T : class
        {
            return await ExecuteWithCondition(c => c.Update(t));
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<bool> Delete<T>(long id) where T : class
        {
            return await ExecuteWithCondition(c =>
            {
                var entity = GetById<T>(id);
                var obj = entity.Result as T;
                return c.Delete(obj);
            });
        }

        /// <summary>
        /// 根据条件批量删除
        /// </summary>
        /// <param name="pg"></param>
        /// <returns></returns>
        public virtual async Task<bool> BatchDelete(PredicateGroup pg)
        {
            return await ExecuteWithCondition(c => c.Delete(pg));
        }

        /// <summary>
        /// 执行一条sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public virtual async Task<int> Execute(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return await ExecuteWithCondition(c => c.Execute(sql, param, transaction, commandTimeout, commandType));
        }

        public virtual async Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return await ExecuteWithConditionAsync(async c => await c.ExecuteAsync(sql, param, transaction, commandTimeout, commandType));
        }

        /// <summary>
        /// 根据id获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> GetById<T>(long id) where T : class
        {
            return await ExecuteWithCondition(c => c.Get<T>(id));
        }

        /// <summary>
        /// 获取总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual async Task<int> GetTotal<T>() where T : class
        {
            return await this.GetTotal<T>(null);
        }

        /// <summary>
        /// 根据条件获取总数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pg"></param>
        /// <returns></returns>
        public virtual async Task<int> GetTotal<T>(PredicateGroup pg) where T : class
        {
            return await ExecuteWithCondition(c => c.Count<T>(pg));
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetList<T>() where T : class
        {
            return await this.GetListWithCondition<T>(null, null);
        }

        /// <summary>
        /// 根据条件获取数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pg"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetListWithCondition<T>(PredicateGroup pg, IList<ISort> sort = null) where T : class
        {
            return await ExecuteWithCondition(c => c.GetList<T>(pg, sort));
        }

        /// <summary>
        /// 根据条件获取数据列表(带分页)
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="pg"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetListWithConditionPageList<T>(int page, int pageSize, PredicateGroup pg, IList<ISort> sort = null) where T : class
        {
            return await ExecuteWithCondition(c => c.GetPage<T>(pg, sort, page, pageSize).ToList());
        }

        protected async Task<T> ExecuteWithCondition<T>(Func<IDbConnection, T> execute, string conn = DataBaseConnection.LOCAL_CONNECTION)
        {
            try
            {
                Configure();
                using (var connection = Infrastructure.Data.Database.Connection(conn))
                {
                    try
                    {
                        await connection.OpenAsync();
                        return execute(connection);
                    }
                    catch (Exception ex)
                    {
                        return default(T);
                    }
                }
            }
            catch (TimeoutException ex)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL timeout", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL exception (not a timeout)", ex);
            }
        }


        protected async Task<T> ExecuteWithConditionAsync<T>(Func<IDbConnection, Task<T>> execute, string conn = DataBaseConnection.LOCAL_CONNECTION)
        {
            try
            {
                Configure();
                using (var connection = Infrastructure.Data.Database.Connection(conn))
                {
                    await connection.OpenAsync();
                    return await execute(connection);
                }
            }
            catch (TimeoutException ex)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL timeout", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL exception (not a timeout)", ex);
            }
        }

        protected void ExecuteWithTransaction<T>(Action<IDbConnection, IDbTransaction> execute, string conn = DataBaseConnection.LOCAL_CONNECTION)
        {
            try
            {
                Configure();
                using (var connection = Infrastructure.Data.Database.Connection(conn))
                {
                    connection.Open();
                    IDbTransaction transaction = connection.BeginTransaction();
                    try
                    {
                        execute(connection, transaction);
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                    finally
                    {
                        transaction.Commit();
                    }
                }
            }
            catch (TimeoutException ex)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL timeout", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL exception (not a timeout)", ex);
            }
        }

        protected int ExecuteWithTransaction(IDbConnection conn, string sql, object param)
        {
            using (IDbTransaction transaction = conn.BeginTransaction())
            {
                int rowCount = 0;
                try
                {
                    rowCount = conn.Execute(sql, param, transaction);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                return rowCount;
            }
        }

        protected async Task<int> ExecuteWithTransactionAsync(IDbConnection conn, string sql, object param)
        {
            using (IDbTransaction transaction = conn.BeginTransaction())
            {
                int rowCount = 0;
                try
                {
                    rowCount = await conn.ExecuteAsync(sql, param, transaction);
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                return rowCount;
            }
        }

        protected T ExecuteWithConditionSync<T>(Func<IDbConnection, T> execute, string conn = DataBaseConnection.LOCAL_CONNECTION)
        {
            try
            {
                Configure();
                using (var connection = Infrastructure.Data.Database.Connection(conn))
                {
                    connection.Open();
                    return execute(connection);
                }
            }
            catch (TimeoutException ex)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL timeout", ex);
            }
            catch (SqlException ex)
            {
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL exception (not a timeout)", ex);
            }
        }

        /// <summary>
        /// 获取表名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected string GetTableName<T>() where T : class
        {
            System.Attribute attr = System.Attribute.GetCustomAttributes(typeof(T))[0];
            var tableName = (attr as dynamic).TableName;
            return tableName;
        }

        /// <summary>
        /// 设置DapperExtension使用mysql拼接sql语句
        /// </summary>
        protected void Configure()
        {
            DapperExtensions.DapperExtensions.Configure(this.Configuration.Value);
        }
    }
}
