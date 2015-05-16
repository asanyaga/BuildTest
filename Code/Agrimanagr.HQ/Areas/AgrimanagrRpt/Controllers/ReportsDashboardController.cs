using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Agrimanagr.HQ.Areas.AgrimanagrRpt.Controllers
{
    public class ReportsDashboardController : Controller
    {
        //
        // GET: /AgrimanagrRpt/ReportsDashboard/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ReportView(string reporturi, string reportName)
        {
            @ViewBag.reporturi = reporturi;
            @ViewBag.ReportName = reportName;
            return View();
        }

    }
}
