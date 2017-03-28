using Disconf.Net.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disconf.Net.Domain.Models;
using Disconf.Net.Domain.Repositories;
using DapperExtensions;
using Disconf.Net.Infrastructure.Helper;
using Disconf.Net.Domain.Condition;

namespace Disconf.Net.Application.Implementation
{
    public class UserServiceImpl : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleService _roleService;
        public UserServiceImpl(IUserRepository userRepository, IRoleService roleService)
        {
            _userRepository = userRepository;
            _roleService = roleService;
        }
        public async Task<bool> Delete(long id)
        {
            return await _userRepository.Delete<User>(id);
        }

        public async Task<User> Get(long id)
        {
            return await _userRepository.GetById<User>(id);
        }

        public async Task<IEnumerable<User>> GetList()
        {
            return await _userRepository.GetList<User>();
        }

        public async Task<bool> Insert(User model)
        {
            return await _userRepository.Insert<User>(model);
        }

        public async Task<bool> Update(User model)
        {
            return await _userRepository.Update<User>(model);
        }
        public async Task<User> Login(User model)
        {
            var user = await GetUserByUserName(model);
            return user;
        }

        public async Task<User> GetUserByUserName(User model)
        {
            var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
            pg.Predicates.Add(Predicates.Field<User>(u => u.UserName, Operator.Eq, model.UserName));
            pg.Predicates.Add(Predicates.Field<User>(u => u.PassWord, Operator.Eq, UtilHelper.Md5(model.PassWord)));
            var list = await _userRepository.GetListWithCondition<User>(pg);
            var user = list.Any() ? list.Where(s => s.IsDelete == false).FirstOrDefault() : null;
            return user;
        }

        public async Task<IEnumerable<User>> GetUserList(long userId)
        {
            var condition = new RoleCondition
            {
                CreateId = userId
            };
            var roleList = await _roleService.GetList(condition);
            var roleidList = string.Join(",", roleList.Select(s => s.Id.ToString()));
            return await _userRepository.GetUserList(roleidList);
        }
    }
}
