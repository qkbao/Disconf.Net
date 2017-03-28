using DapperExtensions;
using Disconf.Net.Application.Interfaces;
using Disconf.Net.Domain.Condition;
using Disconf.Net.Domain.Models;
using Disconf.Net.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Implementation
{
    public class ConfigServiceImpl : IConfigService
    {
        private readonly IConfigRepository _configRepository;
        public ConfigServiceImpl(IConfigRepository configRepository)
        {
            _configRepository = configRepository;
        }
        public async Task<IEnumerable<Configs>> GetList()
        {
            return await _configRepository.GetList<Configs>();
        }
        public async Task<IEnumerable<Configs>> GetList(ConfigCondition condition)
        {
            var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
            if (condition.TemplateId.HasValue)
            {
                pg.Predicates.Add(Predicates.Field<Configs>(l => l.TemplateId, Operator.Eq, condition.TemplateId.Value));
            }
            if (condition.EnvId.HasValue)
            {
                pg.Predicates.Add(Predicates.Field<Configs>(l => l.EnvId, Operator.Eq, condition.EnvId.Value));
            }
            return await _configRepository.GetListWithCondition<Configs>(pg);
        }

        public async Task<bool> Insert(Configs model)
        {
            return await _configRepository.Insert(model);
        }
        public async Task<bool> Update(Configs model)
        {
            return await _configRepository.Update(model);
        }

        public async Task<Configs> Get(long id)
        {
            return await _configRepository.GetById<Configs>(id);
        }

        public async Task<bool> Delete(long id)
        {
            return await _configRepository.Delete<Configs>(id);
        }

        public async Task<IEnumerable<Configs>> GetConfigsByTemplateIds(string ids)
        {
            return await _configRepository.GetConfigsByTemplateIds(ids);
        }
    }
}
