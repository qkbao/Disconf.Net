using DapperExtensions;
using Disconf.Net.Application.Interfaces;
using Disconf.Net.Domain.Enum;
using Disconf.Net.Domain.Models;
using Disconf.Net.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disconf.Net.Application.Implementation
{
    public class PermissionServiceImpl : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IEnvService _envService;
        private readonly IAppService _appService;
        private readonly IRoleService _roleService;
        public PermissionServiceImpl(IPermissionRepository permissionRepository, IEnvService envService, IAppService appService, IRoleService roleService)
        {
            _permissionRepository = permissionRepository;
            _envService = envService;
            _roleService = roleService;
            _appService = appService;
        }

        public async Task<IEnumerable<Permission>> GetList()
        {
            return await _permissionRepository.GetList<Permission>();
        }
        public async Task<IEnumerable<Permission>> GetPers(string name)
        {
            var pg = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };
            if (!string.IsNullOrWhiteSpace(name))
            {
                pg.Predicates.Add(Predicates.Field<Permission>(l => l.Name, Operator.Eq, name));
            }
            return await _permissionRepository.GetListWithCondition<Permission>(pg);
        }

        public async Task<bool> Insert(Permission model)
        {
            return await _permissionRepository.Insert(model);
        }
        public async Task<bool> Update(Permission model)
        {
            return await _permissionRepository.Update(model);
        }

        public async Task<Permission> Get(long id)
        {
            return await _permissionRepository.GetById<Permission>(id);
        }

        public async Task<bool> Delete(long id)
        {
            return await _permissionRepository.Delete<Permission>(id);
        }

        public async Task<bool> BatchInsert(string name)
        {
            var appId = await _appService.GetAppIdByName(name);
            var perModel = new Permission()
            {
                Name = name,
                ParentId = 0,
                AppId = appId,
                PermissionType = (int)PermissionType.App,
                Code = appId + ".0"
            };
            var isPer = await _permissionRepository.Insert(perModel);
            if (isPer)
            {
                var list = await GetPers(name);
                var perId = list.FirstOrDefault().Id;
                var envList = await _envService.GetList();
                foreach (var item in envList)
                {
                    var model = new Permission()
                    {
                        Name = item.Name,
                        AppId = appId,
                        ParentId = perId,
                        PermissionType = (int)PermissionType.Env,
                        Code = appId + "." + item.Id
                    };
                    await _permissionRepository.Insert(model);
                }
            }
            return true;
        }

        public async Task<bool> BatchInsertByAfterEnv(string name)
        {
            var envId = await _envService.GetEnvIdByName(name);
            var perList = await _permissionRepository.GetList<Permission>();
            foreach (var item in perList.Where(s => s.PermissionType == (int)PermissionType.App))
            {
                var appId = item.Code.Split('.')[0];
                var model = new Permission()
                {
                    AppId = long.Parse(appId),
                    Name = name,
                    ParentId = item.Id,
                    PermissionType = (int)PermissionType.Env,
                    Code = appId + "." + envId
                };
                await _permissionRepository.Insert(model);
            }
            return true;
        }

        public async Task<bool> BatchUpdatePer(string oldName, string newName, PermissionType type)
        {
            var pers = await GetPers(oldName);
            foreach (var item in pers)
            {
                item.Name = newName;
                item.PermissionType = (int)type;
                await Update(item);
            }
            return true;
        }

        public async Task<bool> BatchDeleteByAppId(long appId)
        {
            return await _permissionRepository.BatchDeleteByAppId(appId);
        }
        public async Task<bool> BatchDeleteByName(string name)
        {
            return await _permissionRepository.BatchDeleteByName(name);
        }


        /// <summary>
        /// 根据roleId获取权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<List<Permission>> GetPermissionListByRoleId(long roleId)
        {
            var role = await _roleService.Get(roleId);
            var permissionIds = role.PermissionIds.Split('|').ToList();
            if (permissionIds.Any())
            {
                string ids = string.Join(",", permissionIds);
                var list = await _permissionRepository.GetPermissionList(ids);
                return list.ToList();
            }
            return new List<Permission>();
        }

        /// <summary>
        /// 检测某角色是否有权限
        /// </summary>
        /// <param name="code"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<bool> AuthorityCheck(string code, long roleId)
        {
            if (roleId == 1)
                return true;
            var permissionRecords = await GetPermissionListByRoleId(roleId);
            return permissionRecords.Any(permissionRecord => permissionRecord.Code == code);
        }
    }
}
