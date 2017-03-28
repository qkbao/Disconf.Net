using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disconf.Net.Web.Filters
{
    public class ActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = HttpContext.Current.Session["User"];
            if (user == null)
            {
                filterContext.Result = new RedirectResult("/Home/Login");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}