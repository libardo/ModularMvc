﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Modular.Mvc.TestApp.Modules.Management.Users
{
    public class EditUserController : Controller
    {
        //
        // GET: /CreateUser/

        public ActionResult Index(int userId)
        {
            return View();
        }

    }
}
