using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Disconf.Net.Web.Controllers
{
    public class PageController : Controller
    {
        // GET: Page
        public ActionResult NoPermission()
        {
            return View();
        }
    }
}