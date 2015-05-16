using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class OrdersByDistributorController : Controller
    {
        //
        // GET: /Rpt/OrdersByDistributor/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Rpt/OrdersByDistributor/OrderBySalesman

        public ActionResult OrdersBySalesman(Guid? distributorId,DateTime? startDate,DateTime? endDate)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&startDate=" + startDate + "&endDate=" + endDate;
            return View();
        }

        //
        // GET: /Rpt/OrdersByDistributor/OrdersSummary

        public ActionResult OrdersSummary(Guid? distributorId, Guid? salesmanId, Guid? routeId, Guid? outletId, Guid? brandId, DateTime? startDate, DateTime? endDate)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&salesmanId=" + salesmanId + "&routeId=" + routeId + "&outletId=" + outletId + "&brandId=" + brandId + "&startDate=" + startDate + "&endDate=" + endDate;
            return View();
        }

        //
        // GET: /Rpt/OrdersByDistributor/OrdersBySalesmanPerBrand

        public ActionResult OrdersBySalesmanPerBrand(Guid? distributorId, Guid? salesmanId, Guid? routeId, Guid? outletId, Guid? brandId, DateTime? startDate, DateTime? endDate)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&salesmanId=" + salesmanId + "&outletId=" + outletId + "&routeId=" + routeId + "&brandId=" + brandId + "&startDate=" + startDate + "&endDate=" + endDate;
            return View();
        }

        //
        // GET: /Rpt/OrdersByDistributor/OrderDetails
        public ActionResult OrderDetails(Guid? orderId, DateTime? startDate, DateTime? endDate)
        {
            ViewBag.Parameters = "?id=" + orderId  + "&startDate=" + startDate + "&endDate=" + endDate;
            return View();
        }
    }
}
