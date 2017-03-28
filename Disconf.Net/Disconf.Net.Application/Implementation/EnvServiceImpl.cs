using DapperExtensions;
using Disconf.Net.Application.Interfaces;
using Disconf.Net.Domain.Condition;
using Disconf.Net.Domain.Models;
using Disconf.Net.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Implementation
{
    public class EnvServiceImpl : IEnvService
    {
        private readonly IEnvRepository _envRepository;
        public EnvServiceImpl(IEnvRepository envRepository)
        {
            _envRepository = envRepository;
        }

        public async Task<IEnumerable<Envs>> GetList()
        {
            return await _envRepository.GetList<Envs>();
        }

        public async Task<bool> Insert(Envs model)
        {
            return await _envRepository.Insert(model);
        }
        public async Task<bool> Update(Envs model)
        {
            return await _envRepository.Update(model);
        }

        public async Task<Envs> Get(long id)
        {
            return await _envRepository.GetById<Envs>(id);
        }

        public async Task<bool> Delete(long id)
        {
            return await _envRepository.Delete<Envs>(id);
        }

        public async Task<IEnumerable<Envs>> GetList(EnvCondition condition)
        {
            var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
            if (!string.IsNullOrWhiteSpace(condition.Name))
            {
                pg.Predicates.Add(Predicates.Field<Envs>(l => l.Name, Operator.Eq, condition.Name));
            }
            return await _envRepository.GetListWithCondition<Envs>(pg);
        }

        public async Task<long> GetEnvIdByName(string name)
        {
            var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
            if (!string.IsNullOrWhiteSpace(name))
            {
                pg.Predicates.Add(Predicates.Field<Envs>(l => l.Name, Operator.Eq, name));
            }
            var list = await _envRepository.GetListWithCondition<Envs>(pg);
            return list.FirstOrDefault().Id;
        }
        public async Task<string> GetEnvNameById(long id)
        {
            var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
            pg.Predicates.Add(Predicates.Field<Envs>(l => l.Id, Operator.Eq, id));
            var list = await _envRepository.GetListWithCondition<Envs>(pg);
            return list.FirstOrDefault().Name;
        }
    }
}
