using Disconf.Net.Application.Interfaces;
using Disconf.Net.Domain.Condition;
using Disconf.Net.Domain.Enum;
using Disconf.Net.Domain.Models;
using Disconf.Net.Model.Result;
using Disconf.Net.Web.Filters;
using Disconf.Net.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Disconf.Net.Web.Controllers
{
    public class AppController : BaseController
    {
        private readonly IAppService _appService;
        private readonly IPermissionService _permissionService;
        private readonly IEnvService _envService;
        private readonly IRoleService _roleService;
        private readonly ITemplateService _templateService;
        private readonly IConfigService _configService;
        public AppController(IAppService appService, IPermissionService permissionService, IRoleService roleService, IEnvService envService, ITemplateService templateService, IConfigService configService)
        {
            this._appService = appService;
            this._permissionService = permissionService;
            this._roleService = roleService;
            this._envService = envService;
            this._templateService = templateService;
            this._configService = configService;
        }
        #region view
        // GET: App
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }
        #endregion


        public async Task<JsonResult> Get(long id)
        {
            var model = await _appService.Get(id);
            var obj = new
            {
                Name = model.Name,
                Description = model.Description
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        [ActionLogActionFilter(ActionContent = "插入应用")]
        public async Task<JsonResult> Insert(Apps model)
        {
            var result = new BaseResult();
            var list = await _appService.GetList();
            var nameList = list.Where(s => s.Name.Trim() == model.Name);
            if (nameList != null && nameList.Count() > 0)
            {
                result.IsSuccess = false;
                result.ErrorMsg = "该App名称已经存在";
            }
            else
            {
                await _appService.Insert(model);
                result.IsSuccess = await _permissionService.BatchInsert(model.Name);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [ActionLogActionFilter(ActionContent = "更新应用")]
        public async Task<JsonResult> Update(Apps model)
        {
            var result = new BaseResult();
            var list = await _appService.GetList();
            var nameList = list.Where(s => string.Equals(s.Name, model.Name, StringComparison.OrdinalIgnoreCase));
            var oldModel = list.Where(s => s.Id == model.Id).FirstOrDefault();
            if (nameList != null && nameList.Count() > 0 && nameList.FirstOrDefault().Id != model.Id)
            {
                result.IsSuccess = false;
                result.ErrorMsg = "该App名称已经存在";
            }
            else
            {
                await _permissionService.BatchUpdatePer(oldModel.Name, model.Name, PermissionType.App);
                result.IsSuccess = await _appService.Update(model);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [ActionLogActionFilter(ActionContent = "删除应用")]
        public async Task<JsonResult> Delete(long id)
        {
            var result = new BaseResult();
            TemplateCondition condition = new TemplateCondition
            {
                AppId = id
            };
            var temList = await _templateService.GetList(condition);
            if (temList.Count() > 0)
            {
                result.IsSuccess = false;
                result.ErrorMsg = "该App下存在模板，请先删除模板";
            }
            else
            {
                await _permissionService.BatchDeleteByAppId(id);
                result.IsSuccess = await _appService.Delete(id);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetList()
        {
            var model = await _appService.GetList();
            if (model == null || model.Count() == 0)
                return Json(model, JsonRequestBehavior.AllowGet);
            var user = (User)Session["User"];
            var role = await _roleService.Get(user.RoleId);
            var pers = await _permissionService.GetList();

            var appPers = role.Id.Equals(1) ? pers : pers.Where(s => s.PermissionType == (int)PermissionType.App && role.PermissionIds.Split('|').Contains(s.Id.ToString())).ToList();
            var list = model.Where(s => appPers.Select(t => t.AppId).ToList().Contains(s.Id)).Select(s => new
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description
            }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> InitZookeeper(int id, string appName)
        {
            try
            {
                TemplateCondition condition = new TemplateCondition
                {
                    AppId = id
                };
                var temList = await _templateService.GetList(condition);
                if (temList != null && temList.Count() != 0)
                {
                    var configList = await _configService.GetConfigsByTemplateIds(string.Join(",", temList.Select(s => s.Id)));
                    foreach (var item in configList)
                    {
                        var tem = temList.Where(s => s.Id == item.TemplateId).FirstOrDefault();
                        var envName = await _envService.GetEnvNameById(item.EnvId);
                        var path = DisconfWatcher.GetPath(tem.Name, appName, tem.Version, envName, tem.Type);
                        DisconfWatcher.AddOrSetData(path);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}