using DapperExtensions;
using Disconf.Net.Application.Interfaces;
using Disconf.Net.Domain.Models;
using Disconf.Net.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Implementation
{
    public class AppServiceImpl : IAppService
    {
        private readonly IAppRepository _appRepository;
        public AppServiceImpl(IAppRepository appRepository)
        {
            _appRepository = appRepository;
        }

        public async Task<IEnumerable<Apps>> GetList()
        {
            return await _appRepository.GetList<Apps>();
        }

        public async Task<bool> Insert(Apps model)
        {
            return await _appRepository.Insert(model);
        }
        public async Task<bool> Update(Apps model)
        {
            return await _appRepository.Update(model);
        }

        public async Task<Apps> Get(long id)
        {
            return await _appRepository.GetById<Apps>(id);
        }

        public async Task<bool> Delete(long id)
        {
            return await _appRepository.Delete<Apps>(id);
        }

        public async Task<long> GetAppIdByName(string name)
        {
            var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
            if (!string.IsNullOrWhiteSpace(name))
            {
                pg.Predicates.Add(Predicates.Field<Apps>(l => l.Name, Operator.Eq, name));
            }
            var list = await _appRepository.GetListWithCondition<Apps>(pg);
            return list.FirstOrDefault().Id;
        }
        public async Task<string> GetAppNameById(long id)
        {
            var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
            pg.Predicates.Add(Predicates.Field<Apps>(l => l.Id, Operator.Eq, id));
            var list = await _appRepository.GetListWithCondition<Apps>(pg);
            return list.FirstOrDefault().Name;
        }
    }
}
