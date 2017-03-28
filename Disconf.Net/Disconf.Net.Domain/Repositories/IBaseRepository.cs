using DapperExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Domain.Repositories
{
    public interface IBaseRepository
    {
        Task<bool> Insert<T>(T t) where T : class;

        Task<bool> Update<T>(T t) where T : class;

        Task<bool> Delete<T>(long id) where T : class;

        Task<bool> BatchDelete(PredicateGroup pg);

        Task<int> Execute(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

        Task<T> GetById<T>(long id) where T : class;

        Task<int> GetTotal<T>() where T : class;

        Task<int> GetTotal<T>(PredicateGroup pg) where T : class;

        Task<IEnumerable<T>> GetList<T>() where T : class;

        Task<IEnumerable<T>> GetListWithCondition<T>(PredicateGroup pg, IList<ISort> sort = null) where T : class;

        Task<IEnumerable<T>> GetListWithConditionPageList<T>(int page, int pageSize, PredicateGroup pg, IList<ISort> sort = null) where T : class;
    }
}
