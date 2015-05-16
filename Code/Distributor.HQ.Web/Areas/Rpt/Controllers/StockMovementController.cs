using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class StockMovementController : Controller
    {
        //
        // GET: /Rpt/StockMovement/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetStockByBrand(Guid? distributorId, DateTime startDate, DateTime endDate)
        {
            ViewBag.Parameters = "?DistributorId=" + distributorId + "&startDate=" + startDate + "&endDate=" + endDate; 
            return View();
        }

        public ActionResult GetStockByProduct(Guid? distributorId, Guid? brandId, DateTime startDate, DateTime endDate)
        {
            ViewBag.Parameters = "?DistributorId=" + distributorId + "&BrandId=" + brandId +"&startDate=" + startDate + "&endDate=" + endDate; 
            return View();
        }

        public ActionResult GetStockSummary(Guid? distributorId, Guid? brandId, Guid? productId, DateTime startDate, DateTime endDate)
        {
            ViewBag.Parameters = "?DistributorId=" + distributorId + "&BrandId=" + brandId + "&productId=" + productId + "&startDate=" + startDate + "&endDate=" + endDate; 
            return View();
        }
    }
}
