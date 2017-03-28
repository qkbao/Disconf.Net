using Disconf.Net.Application.Interfaces;
using Disconf.Net.Domain.Models;
using Disconf.Net.Infrastructure.Helper;
using Disconf.Net.Model.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Disconf.Net.Web.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        public UserController(IUserService userService, IRoleService roleService)
        {
            this._userService = userService;
            this._roleService = roleService;
        }
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Edit()
        {
            return View();
        }
        public ActionResult Add()
        {
            return View();
        }
        public async Task<JsonResult> Get(long id)
        {
            var model = await _userService.Get(id);
            var list = await _roleService.GetList();
            var obj = new
            {
                Name = model.Name,
                UserName = model.UserName,
                PassWord = model.PassWord,
                IsSystem = model.IsSystem,
                RoleId = model.RoleId,
                Roles = list.Select(s => new
                {
                    Name = s.Name,
                    Id = s.Id
                }).ToList()
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> Insert(User model)
        {
            var result = new BaseResult();
            var list = await _userService.GetList();
            var nameList = list.Where(s => s.UserName == model.UserName);
            if (nameList != null && nameList.Count() > 0)
            {
                result.IsSuccess = false;
                result.ErrorMsg = "该用户名已经存在";
            }
            else
            {
                model.PassWord = UtilHelper.Md5(model.PassWord);
                result.IsSuccess = await _userService.Insert(model);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetList()
        {
            var user = (User)Session["User"];
            var list = await _userService.GetUserList(user.Id);
            return Json(list.Where(s => s.Id > 1), JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> Update(User model)
        {
            var result = new BaseResult();
            var list = await _userService.GetList();
            var nameList = list.Where(s => s.UserName == model.UserName);
            if (nameList != null && nameList.Count() > 0 && nameList.FirstOrDefault().Id != model.Id)
            {
                result.IsSuccess = false;
                result.ErrorMsg = "该用户名已经存在";
            }
            else
            {
                var user = list.Where(s => s.Id == model.Id).FirstOrDefault();
                if (user.PassWord != model.PassWord)
                    model.PassWord = UtilHelper.Md5(model.PassWord);
                result.IsSuccess = await _userService.Update(model);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Delete(long id)
        {
            await _userService.Delete(id);
            return RedirectToAction("Index");
        }

    }
}