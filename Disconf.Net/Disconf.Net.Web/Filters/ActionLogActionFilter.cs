using Disconf.Net.Domain.Models;
using Disconf.Net.Web.App_Start;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Disconf.Net.Web.Filters
{
    public class ActionLogActionFilter : ActionFilterAttribute
    {
        public string ActionContent { get; set; }
        public bool IgnoreDetails { get; set; }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            User user = HttpContext.Current.Session["user"] as User;

            if (user != null)
            {//只有用户登录后才做日志
                string routeInfo = GetRouteData(filterContext.RouteData);
                string requestInfo = GetRequestData(filterContext);
                var log = new OperationLog();
                log.UId = user.Id;
                log.Content = IgnoreDetails ? $"{ActionContent} {routeInfo}" : $"{ActionContent} {requestInfo} {routeInfo}";
                log.Ctime = DateTime.Now;
                LogQueue.AppLogQueue.Enqueue(log);
            }
            base.OnActionExecuted(filterContext);
        }
        private string GetRequestData(ActionExecutedContext filterContext)
        {
            //获取所有的请求参数
            StringBuilder tmp = new StringBuilder();
            foreach (var kv in filterContext.RouteData.Values)
            {
                if (!string.Equals("controller", kv.Key, StringComparison.OrdinalIgnoreCase)
                    && !string.Equals("action", kv.Key, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(kv.Value?.ToString()))
                {
                    tmp.Append(kv.Key);
                    tmp.Append(':');
                    tmp.Append(kv.Value);
                    tmp.Append(';');
                }
            }
            Action<NameValueCollection> act = (collection) =>
            {
                if (collection != null && collection.Count > 0)
                {
                    foreach (string key in collection.Keys)
                    {
                        if (!string.IsNullOrWhiteSpace(collection[key]))
                        {
                            tmp.Append(key);
                            tmp.Append(':');
                            tmp.Append(collection[key]);
                            tmp.Append(';');
                        }
                    }
                }
            };
            act(filterContext.HttpContext.Request.QueryString);
            act(filterContext.HttpContext.Request.Form);
            if (tmp.Length > 0)
            {
                tmp.Insert(0, "请求内容：");
            }
            return tmp.ToString();
        }
        private string GetRouteData(RouteData routeData)
        {
            var controller = routeData.Values["controller"];
            var action = routeData.Values["action"];
            return $"Controller:{controller}; Action:{action};";
        }
    }
}