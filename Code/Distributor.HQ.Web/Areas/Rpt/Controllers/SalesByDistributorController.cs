using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class SalesByDistributorController : Controller
    {
        //
        // GET: /Rpt/SalesByDistributor/

        public ActionResult SaleByDistributor(Guid? distributorId, DateTime? startDate, DateTime? endDate)
        {
            return View();
        }

        public ActionResult SaleBySalesman(Guid? distributorId, Guid? salesmanId, DateTime? startDate, DateTime? endDate)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&salesmanId=" + salesmanId + "&startDate=" + startDate + "&endDate=" +
          endDate;
            return View();
        }

        public ActionResult SalesBySalesmanPerBrand(Guid? distributorId, Guid? salesmanId, Guid? routeId, Guid? outletId, Guid? brandId, DateTime? startDate, DateTime? endDate)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&salesmanId=" + salesmanId +/* "&routeId=" + routeId + "&outletId=" + outletId +*/ "&brandId=" + brandId +"&startDate=" + startDate + "&endDate=" + endDate;
            return View();
        }

        public ActionResult SaleSummary(Guid? distributorId, Guid? salesmanId, Guid? routeId, Guid? outletId, Guid? brandId, DateTime? startDate, DateTime? endDate)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&salesmanId=" + salesmanId + "&routeId=" + routeId + "&outletId=" + outletId + "&brandId=" + brandId + "&startDate=" + startDate + "&endDate=" +
         endDate;
            return View();
        }

    }
}
