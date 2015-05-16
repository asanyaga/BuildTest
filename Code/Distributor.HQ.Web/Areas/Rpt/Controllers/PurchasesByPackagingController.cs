using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class PurchasesByPackagingController : Controller
    {
        //
        // GET: /Rpt/PurchasesByPackaging/

        public ActionResult Index()
        {
            return View();
        }

        // GET: /Rpt/PurchasesByPackaging/ProductPurchasesPerPackaging

        public ActionResult ProductPurchasePerPackaging(Guid packageId,DateTime? startDate,DateTime? endDate)
        {
            ViewBag.Parameters = "?packageId=" + packageId + "&startDate=" + startDate + "&endDate=" + endDate;

            return View();
        }

    }
}
