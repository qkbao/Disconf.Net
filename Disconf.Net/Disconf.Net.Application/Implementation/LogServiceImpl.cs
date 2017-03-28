using DapperExtensions;
using Disconf.Net.Application.Interfaces;
using Disconf.Net.Domain.Models;
using Disconf.Net.Domain.Repositories;
using Disconf.Net.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Implementation
{
    public class LogServiceImpl : ILogService
    {
        private readonly ILogRepository _logRepository;

        public LogServiceImpl(ILogRepository logRepository)
        {
            this._logRepository = logRepository;
        }

        public async Task<IEnumerable<OperationLog>> GetLogList(int pageSize, LogPagingFilteringVM command)
        {
            var pg = this.GetPredicateGroup(command);
            var sort = new List<ISort>();
            sort.Add(new Sort { PropertyName = "Id", Ascending = false });
            return await this._logRepository.GetListWithConditionPageList<OperationLog>(command.Page - 1, pageSize, pg, sort);
        }

        public async Task<int> GetLogTotal(LogPagingFilteringVM command)
        {
            var pg = this.GetPredicateGroup(command);
            return await this._logRepository.GetTotal<OperationLog>(pg);
        }

        public async Task<bool> Insert(OperationLog log)
        {
            var result = await _logRepository.Insert(log);
            return result;
        }

        public Task<bool> Update(OperationLog log)
        {
            throw new NotSupportedException();
        }

        public async Task InsertBatch(IEnumerable<OperationLog> logs)
        {
            if (logs != null && logs.Any())
            {
                await this._logRepository.InsertBatch(logs);
            }
        }

        #region private function
        /// <summary>
        /// 获取DapperExtensions的PredicateGroup
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private PredicateGroup GetPredicateGroup(LogPagingFilteringVM command)
        {
            var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
            if ((command?.StartTime).HasValue)
            {
                pg.Predicates.Add(Predicates.Field<OperationLog>(l => l.Ctime, Operator.Ge, command.StartTime));
            }
            if ((command?.EndTime).HasValue)
            {
                pg.Predicates.Add(Predicates.Field<OperationLog>(l => l.Ctime, Operator.Lt, command.EndTime.Value.AddDays(1)));
            }
            if (!string.IsNullOrWhiteSpace(command?.Content))
            {
                pg.Predicates.Add(Predicates.Field<OperationLog>(l => l.Content, Operator.Like, $"{command.Content.Trim()}%"));
            }
            return pg;
        }
        #endregion

        #region NotSupportedException
        public Task<bool> Delete(long id)
        {
            throw new NotSupportedException();
        }

        public Task<OperationLog> GetLog(long id)
        {
            throw new NotSupportedException();
        }

        public Task<IEnumerable<OperationLog>> GetLogList()
        {
            throw new NotSupportedException();
        }
        #endregion 
    }
}
