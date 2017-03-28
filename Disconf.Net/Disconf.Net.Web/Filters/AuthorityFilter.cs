using Disconf.Net.Application.Interfaces;
using Disconf.Net.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disconf.Net.Web.Filters
{
    public class AuthorityFilter : ActionFilterAttribute
    {
        public string Code { set; get; }//要验证的权限的代码

        public override async void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var user = (User)HttpContext.Current.Session["User"];
            //判断是否有权限
            var hasAuthority = await this.PermissionService.AuthorityCheck(Code, user.RoleId);
            if (!hasAuthority)
            {
                var url = "/Page/NoPermission";
                filterContext.Result = new RedirectResult(url);
                return;
            }

            base.OnActionExecuting(filterContext);
        }
        public IPermissionService PermissionService { get; set; }
    }
}