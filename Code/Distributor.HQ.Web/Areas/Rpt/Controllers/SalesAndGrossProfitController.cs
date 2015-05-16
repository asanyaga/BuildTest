using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class SalesAndGrossProfitController : Controller
    {
        //
        // GET: /Rpt/SalesAndGrossProfit/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DistributorSalesAndGrossProfit(Guid? distributorId, DateTime startDate, DateTime endDate)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&startDate=" + startDate + "&endDate=" + endDate;
            return View();
        }
    }
}
