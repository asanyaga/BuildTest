using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class ReportsController : Controller
    {
        //
        // GET: /Rpt/Reports/

        public ActionResult Index(string reportName)
        {
            ViewBag.ReportName = reportName;
            return View();
        }
        public ActionResult Dashboard(string reportName)
        {
            ViewBag.ReportName = reportName;
            return View();
        }

    }
}
