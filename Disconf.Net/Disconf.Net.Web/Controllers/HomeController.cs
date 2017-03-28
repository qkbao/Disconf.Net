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
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        public HomeController(IUserService userService)
        {
            _userService = userService;
        }
        public ActionResult Login()
        {
            return View();
        }

        public async Task<JsonResult> UserLogin(User user)
        {
            var result = new BaseResult();
            var model = await _userService.Login(user);
            result.IsSuccess = model == null ? false : true;
            if (result.IsSuccess)
                Session["User"] = model;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}