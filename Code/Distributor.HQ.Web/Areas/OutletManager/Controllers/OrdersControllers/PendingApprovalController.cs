﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.OutletManager.Controllers.OrdersControllers
{
    public class PendingApprovalController : Controller
    {
        //
        // GET: /OutletManager/PendingApproval/

        public ActionResult Index()
        {
            return View();
        }

    }
}
