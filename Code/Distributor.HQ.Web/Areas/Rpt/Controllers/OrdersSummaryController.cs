using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class OrdersSummaryController : Controller
    {
        //
        // GET: /Rpt/OrdersSummary/

        public ActionResult Index()
        {
            return View();
        }

        // GET: /Rpt/OrdersSummary/OrdersByDistributor/

        public ActionResult OrdersByDistributor(DateTime? startDate, DateTime? endDate, Guid? distributorId)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&startDate=" + startDate + "&endDate=" + 
                endDate;

            return View();
        }

        // GET: /Rpt/OrdersSummary/OrdersBySalesman/

        public ActionResult OrdersBySalesman(DateTime? startDate, DateTime? endDate, Guid? distributorId, Guid? salesmanId)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&salesmanId=" + salesmanId + "&startDate=" + 
                startDate + "&endDate=" + endDate;

            return View();
        }

        // GET: /Rpt/OrdersSummary/OrdersBySalesmanPerBrand/

        public ActionResult OrdersBySalesmanPerBrand(DateTime? startDate, DateTime? endDate, Guid? distributorId, Guid? salesmanId, Guid brandId)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&salesmanId=" + salesmanId + "&brandId=" + 
                brandId + "&startDate=" + startDate + "&endDate=" + endDate;

            return View();
        }

        // GET: /Rpt/OrdersSummary/OrdersDetails/

        public ActionResult OrdersDetails(Guid documentId)
        {
            ViewBag.Parameters = "?id=" + documentId;

            return View();
        }
    }
}
