using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class SampleController : Controller
    {
        //
        // GET: /Rpt/Sample/

        public ActionResult Index()
        {
            ViewBag.Parameters = "?DistributorID="+Guid.NewGuid()+"&name=Chalo&age=24";
            return View();
        }

    }
}
