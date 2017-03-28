using System.Web.Mvc;

namespace Disconf.Net.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            Elmah.Mvc.Bootstrap.Initialize();
            filters.Add(new HandleErrorAttribute());
        }
    }
}
