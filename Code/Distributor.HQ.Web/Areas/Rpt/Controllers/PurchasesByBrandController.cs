using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class PurchasesByBrandController : Controller
    {
        //
        // GET: /Rpt/DistributorPurchase/

        public ActionResult Index()
        {
            return View();
            
        }

        //
        // GET: /Rpt/DistributorPurchase/ProductPurchasesPerBrand

        public ActionResult ProductPurchasesPerBrand(Guid? brandId, DateTime? startDate, DateTime? endDate)
        {
            ViewBag.Parameters = "?brandId=" + brandId + "&startDate=" + startDate + "&endDate=" + endDate;
            return View();
        }
    }

 
}
