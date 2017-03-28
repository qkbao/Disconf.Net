using Disconf.Net.Application.Interfaces;
using Disconf.Net.Domain.Models;
using Disconf.Net.Model.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Disconf.Net.Web.Filters;
using Disconf.Net.Domain.Enum;

namespace Disconf.Net.Web.Controllers
{
    public class EnvController : BaseController
    {
        private readonly IEnvService _envService;
        private readonly IPermissionService _permissionService;
        private readonly IRoleService _roleService;
        public EnvController(IEnvService envService, IPermissionService permissionService, IRoleService roleService)
        {
            this._envService = envService;
            this._permissionService = permissionService;
            this._roleService = roleService;
        }
        #region view
        // GET: Env
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

        public async Task<ActionResult> Menu()
        {
            var user = (User)Session["User"];
            var role = await _roleService.Get(user.RoleId);
            var pers = await _permissionService.GetList();
            var envPers = role.Id.Equals(1) ? pers : pers.Where(s => s.PermissionType == (int)PermissionType.Env && s.AppId == AppId && role.PermissionIds.Split('|').Contains(s.Id.ToString())).ToList();
            var model = await _envService.GetList();
            return PartialView(model.Where(s => envPers.Select(t => t.Code.Split('.')[1]).ToList().Contains(s.Id.ToString())).ToList());
        }

        public async Task<JsonResult> GetEnvList()
        {
            var model = await _envService.GetList();
            var list = model.Select(s => new
            {
                Name = s.Name,
                Description = s.Description,
                Id = s.Id
            }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> Get(long id)
        {
            var model = await _envService.Get(id);
            var obj = new
            {
                Name = model.Name,
                Description = model.Description,
                Id = model.Id
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> Insert(Envs model)
        {
            var result = new BaseResult();
            var list = await _envService.GetList();
            var nameList = list.Where(s => string.Equals(s.Name.Trim(), model.Name, StringComparison.OrdinalIgnoreCase));
            if (nameList != null && nameList.Count() > 0)
            {
                result.IsSuccess = false;
                result.ErrorMsg = "该环境名称已经存在";
            }
            else
            {
                await _envService.Insert(model);
                result.IsSuccess = await _permissionService.BatchInsertByAfterEnv(model.Name);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> Update(Envs model)
        {
            var result = new BaseResult();
            var list = await _envService.GetList();
            var nameList = list.Where(s => s.Name == model.Name);
            var oldModel = list.Where(s => s.Id == model.Id).FirstOrDefault();
            if (nameList != null && nameList.Count() > 0 && nameList.FirstOrDefault().Id != model.Id)
            {
                result.IsSuccess = false;
                result.ErrorMsg = "该环境名称已经存在";
            }
            else
            {
                await _permissionService.BatchUpdatePer(oldModel.Name, model.Name, PermissionType.Env);
                result.IsSuccess = await _envService.Update(model);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Delete(long id)
        {
            var model = await _envService.Get(id);
            await _permissionService.BatchDeleteByName(model.Name);
            await _envService.Delete(id);
            return RedirectToAction("Index");
        }
    }
}