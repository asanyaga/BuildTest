using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings.DistributorManagement
{
     [Authorize]
    public class SurveyorController : Controller
    {
        //
        // GET: /Admin/Surveyor/

        public ActionResult Index()
        {
            return View();
        }

    }
}
