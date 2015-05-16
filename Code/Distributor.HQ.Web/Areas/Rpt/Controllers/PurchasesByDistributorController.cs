using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class PurchasesByDistributorController : Controller
    {
        //
        // GET: /Rpt/PurchasesByDistributor/

        public ActionResult Index()
        {
            return View();
        }

        // GET: /Rpt/PurchasesByDistributor/PurchasesByOutlet

        public ActionResult PurchasesByOutlet(Guid distributorId, DateTime startDate, DateTime endDate)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&startDate=" + startDate + "&endDate=" + endDate;

            return View();
        }
    }
}
