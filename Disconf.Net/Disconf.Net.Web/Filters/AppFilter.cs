using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disconf.Net.Web.Filters
{
    public class AppFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var appId = HttpContext.Current.Session["AppId"];
            if (appId == null)
            {
                filterContext.Result = new RedirectResult("/App/Index");
            }
            base.OnActionExecuted(filterContext);
        }
    }
}