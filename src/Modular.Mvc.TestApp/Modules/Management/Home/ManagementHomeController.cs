using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Modular.Mvc.TestApp.Modules.Management.Home
{
    public class ManagementHomeController : Controller
    {
        //
        // GET: /Management/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Next(int? page)
        {
            return View(page ?? 0);
        }
    }
}
