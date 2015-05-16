using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings.DistributorManagement
{
     [Authorize]
    public class SalesRepController : Controller
    {
        //
        // GET: /Admin/SalesRep/

        public ActionResult Index()
        {
            return View();
        }

    }
}
