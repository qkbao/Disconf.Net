using Disconf.Net.Application.Interfaces;
using Disconf.Net.Domain.Condition;
using Disconf.Net.Domain.Models;
using Disconf.Net.Model.Result;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Disconf.Net.Web.Filters;
using Disconf.Net.Model.ViewModel;
using System.IO.Compression;
using Disconf.Net.Infrastructure.Helper;
using Disconf.Net.Core.Model;
using Disconf.Net.Web.Models;

namespace Disconf.Net.Web.Controllers
{
    public class TemplateController : BaseController
    {
        private readonly ITemplateService _templateService;
        private readonly IAppService _appService;
        private readonly IEnvService _envService;
        private readonly IConfigService _configService;
        public TemplateController(ITemplateService templateService, IConfigService configService, IAppService appService, IEnvService envService)
        {
            this._templateService = templateService;
            this._appService = appService;
            this._envService = envService;
            this._configService = configService;
        }
        public ActionResult Index(int? appId)
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

        public async Task<JsonResult> GetList()
        {
            var condition = new TemplateCondition();
            condition.AppId = AppId;
            var model = await _templateService.GetList(condition);
            var list = model.Select(s => new
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                DefaultValue = s.DefaultValue,
                Version = s.Version,
                Type = s.Type
            }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetVersion()
        {
            var condition = new TemplateCondition();
            condition.AppId = AppId;
            var model = await _templateService.GetList(condition);
            var list = model.GroupBy(s => s.Version).Select(s => s.FirstOrDefault()).Select(s => new
            {
                Id = s.Id,
                Version = s.Version,
            }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> Get(long id)
        {
            var model = await _templateService.Get(id);
            var obj = new
            {
                Name = model.Name,
                Description = model.Description,
                Id = model.Id,
                DefaultValue = model.DefaultValue,
                Type = model.Type,
                Version = model.Version,

            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        [ActionLogActionFilter(ActionContent = "插入模板")]
        public async Task<JsonResult> Insert(Templates model)
        {
            var result = new BaseResult();
            var condition = new TemplateCondition();
            condition.Name = model.Name;
            condition.Version = model.Version;
            condition.AppId = AppId;
            var list = await _templateService.GetTemplateList(condition);
            if (list.Count() > 0)
            {
                result.IsSuccess = false;
                result.ErrorMsg = "该App下同版本名称已经存在";
            }
            else
            {
                model.AppId = AppId;
                result.IsSuccess = await _templateService.Insert(model);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [ActionLogActionFilter(ActionContent = "更新模板")]
        public async Task<JsonResult> Update(Templates model)
        {
            var result = new BaseResult();
            var condition = new TemplateCondition();
            condition.Name = model.Name;
            condition.Version = model.Version;
            condition.AppId = AppId;
            var list = await _templateService.GetTemplateList(condition);
            if (list.Count() > 0 && list.FirstOrDefault().Id != model.Id)
            {
                result.IsSuccess = false;
                result.ErrorMsg = "该App下同版本名称已经存在";
            }
            else
            {
                model.AppId = AppId;
                result.IsSuccess = await _templateService.Update(model);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [ActionLogActionFilter(ActionContent = "删除模板")]
        public async Task<JsonResult> Delete(long id)
        {
            var condition = new ConfigCondition
            {
                TemplateId = id
            };
            var list = await _configService.GetList(condition);
            if (list != null && list.Count() > 0)
                return Json(false, JsonRequestBehavior.AllowGet);
            else
                return Json(await _templateService.Delete(id), JsonRequestBehavior.AllowGet);
        }


        public JsonResult FileUpload(string qqfileName)
        {
            var result = new QueryResult<string>();

            if (!CheckFileBase(result))
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            var text = ReadFile(Request.Files[0].InputStream);
            result.Data = text;
            result.IsSuccess = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>  
        /// 读文件
        /// </summary>   
        /// <param name="path">文件路径</param>  
        /// <returns></returns>  
        private string ReadFile(Stream stream)
        {
            try
            {
                StreamReader sr = new StreamReader(stream, System.Text.Encoding.GetEncoding("utf-8"));
                string content = sr.ReadToEnd().ToString();
                sr.Close();
                return content;
            }
            catch
            {
                return "<span style='color:red; font-size:x-large;'>Sorry,The Ariticle wasn't found!! It may have been deleted accidentally from Server.</span>";
            }
        }

        private bool CheckFileBase(QueryResult<string> result)
        {
            bool ret = new bool();

            if (Request == null || Request.Files == null
                || Request.Files.Count != 1 || Request.Files[0] == null)
            {
                result.ErrorMsg = "错误的上传文件请求";
                result.IsSuccess = false;
                ret = false;
            }
            else
            {
                ret = true;
            }

            return ret;
        }

        public async Task<JsonResult> GetTemplateList()
        {
            var condition = new TemplateCondition();
            condition.AppId = AppId;
            condition.EnvId = EnvId;
            var list = await _templateService.GetTemplateList(condition);
            var appName = await _appService.GetAppNameById(AppId);
            var envName = await _envService.GetEnvNameById(EnvId);

            foreach (var s in list)
            {
                try
                {
                    var path = DisconfWatcher.GetPath(s.Name, appName, s.Version, envName, s.Type);
                    s.ZookeeperChildren = string.Join("<br>", DisconfWatcher.GetChildren(path));
                }
                catch (Exception ex)
                {

                }
            }
            var vmModel = list.OrderBy(s => s.Version).GroupBy(s => s.Version).Select(t => new TemplateView
            {
                List = t.ToList(),
                Version = t.FirstOrDefault().Version,
            }).ToList();
            return Json(vmModel, JsonRequestBehavior.AllowGet);
        }



        public async Task<ActionResult> DownloadZip(string version)
        {
            var condition = new TemplateCondition
            {
                AppId = AppId,
                EnvId = EnvId,
                Type = (int)ConfigType.File,
                Version = version
            };
            await _templateService.BatchCreateFile(condition);
            var fileName = DateTime.Now.ToString("yyMMddHHmmssff");
            string[] files = Directory.GetFiles(AppSettingHelper.Get<string>("FilePath")); //返回指定目录下的文件名
            Zip(files, fileName);
            return File(AppSettingHelper.Get<string>("ZipPath") + fileName + ".zip", "application/zip", fileName + ".zip");
        }
    }
}