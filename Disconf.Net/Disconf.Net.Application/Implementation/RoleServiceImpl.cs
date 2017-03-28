using DapperExtensions;
using Disconf.Net.Application.Interfaces;
using Disconf.Net.Domain.Condition;
using Disconf.Net.Domain.Models;
using Disconf.Net.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Implementation
{
    public class RoleServiceImpl : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        public RoleServiceImpl(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<IEnumerable<Role>> GetList()
        {
            return await _roleRepository.GetList<Role>();
        }
        public async Task<IEnumerable<Role>> GetList(RoleCondition condition)
        {
            var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
            if (condition.CreateId.HasValue && condition.CreateId > 0 && condition.RoleId > 1)
            {
                pg.Predicates.Add(Predicates.Field<Role>(l => l.CreateId, Operator.Eq, condition.CreateId.Value));
            }
            return await _roleRepository.GetListWithCondition<Role>(pg);
        }

        public async Task<bool> Insert(Role model)
        {
            return await _roleRepository.Insert(model);
        }
        public async Task<bool> Update(Role model)
        {
            return await _roleRepository.Update(model);
        }

        public async Task<Role> Get(long id)
        {
            return await _roleRepository.GetById<Role>(id);
        }

        public async Task<bool> Delete(long id)
        {
            return await _roleRepository.Delete<Role>(id);
        }
    }
}
