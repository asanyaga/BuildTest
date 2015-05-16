using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings
{
     [Authorize]
    public class AsmManagementController : Controller
    {
        //
        // GET: /Admin/DistributorManagement/

        public ActionResult Index()
        {
            return View();
        }

    }
}
