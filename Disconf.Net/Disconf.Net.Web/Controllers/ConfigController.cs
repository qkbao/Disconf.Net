using Disconf.Net.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Disconf.Net.Web.Filters;
using Disconf.Net.Domain.Condition;
using Disconf.Net.Model.Result;
using Disconf.Net.Domain.Models;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Disconf.Net.Web.Models;
using Disconf.Net.Infrastructure;

namespace Disconf.Net.Web.Controllers
{
    public class ConfigController : BaseController
    {
        private readonly IConfigService _configService;
        private readonly IAppService _appService;
        private readonly IEnvService _envService;
        private readonly IPermissionService _permissionService;
        private readonly ITemplateService _templateService;
        public ConfigController(IConfigService configService, IPermissionService permissionService, IAppService appService, IEnvService envService, ITemplateService templateService)
        {
            this._configService = configService;
            this._permissionService = permissionService;
            this._appService = appService;
            this._envService = envService;
            this._templateService = templateService;
        }
        public async Task<ActionResult> Index(int? appId)
        {
            if (appId.HasValue)
            {
                var hasAuthority = await _permissionService.AuthorityCheck(appId.Value + ".0", ((User)Session["User"]).RoleId);
                if (!hasAuthority)
                {
                    return Redirect("/Page/NoPermission");
                }
                Session["AppId"] = appId;
            }
            return View();
        }
        public ActionResult Edit()
        {
            return View();
        }

        public async Task<ActionResult> List(int id)
        {
            var hasAuthority = await _permissionService.AuthorityCheck(id + ".0", ((User)Session["User"]).RoleId);
            if (!hasAuthority)
            {
                return Redirect("/Page/NoPermission");
            }
            Session["EnvId"] = id;
            return View();
        }

        public async Task<JsonResult> GetList()
        {
            var model = await _configService.GetList();
            var list = model.Select(s => new
            {
                Id = s.Id
            }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> UpdateConfig(long? templateId, string value, string version, string name, int type)
        {
            var result = new QueryResult<long>();
            if (!templateId.HasValue)
            {
                result.IsSuccess = false;
                result.ErrorMsg = "错误，请重新进入编辑页面";
            }
            else
            {
                var log = new OperationLog();
                var condition = new ConfigCondition
                {
                    EnvId = EnvId,
                    TemplateId = templateId
                };
                var list = await _configService.GetList(condition);
                if (list != null && list.ToList().Count > 0)
                {
                    var obj = list.FirstOrDefault();
                    obj.Value = value;
                    log.Content = "更新配置 请求内容：更新" + obj.Value + "变更为" + value;
                    obj.Mtime = DateTime.Now;
                    result.IsSuccess = await _configService.Update(obj);
                    DisconfigCache.UpdateCache(AppId + version + EnvId, value, type, name);
                }
                else
                {
                    var obj = new Configs
                    {
                        TemplateId = templateId.Value,
                        EnvId = EnvId,
                        Value = value
                    };
                    log.Content = "启用配置 请求内容：启用;" + value;
                    result.IsSuccess = await _configService.Insert(obj);
                    DisconfigCache.AddCache(AppId + version + EnvId, value, type, name);
                }
                ConfigLog(log);
                result.Data = EnvId;
                var appName = await _appService.GetAppNameById(AppId);
                var envName = await _envService.GetEnvNameById(EnvId);
                var path = DisconfWatcher.GetPath(name, appName, version, envName, type);
                DisconfWatcher.AddOrSetData(path);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetConfigByTemplateId(long templateId)
        {
            var condition = new ConfigCondition
            {
                TemplateId = templateId,
                EnvId = EnvId
            };
            var list = await _configService.GetList(condition);
            var model = list.FirstOrDefault();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> Delete(long id)
        {
            var config = await _configService.Get(id);
            var tem = await _templateService.Get(config.TemplateId);
            DisconfigCache.DeleteCache(AppId + tem.Version + EnvId, tem.Type, tem.Name);
            var log = new OperationLog();
            log.Content = "删除配置 请求内容：删除;" + config.Value;
            ConfigLog(log);
            return Json(await _configService.Delete(id), JsonRequestBehavior.AllowGet);
        }

        private void ConfigLog<T>(T t)
        {
            dynamic model = t;
            model.UId = ((User)HttpContext.Session["user"]).Id;
            model.Ctime = DateTime.Now;
            LogQueue.AppLogQueue.Enqueue(model);
        }
    }
}