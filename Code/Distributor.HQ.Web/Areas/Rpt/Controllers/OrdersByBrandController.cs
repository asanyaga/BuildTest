using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class OrdersByBrandController : Controller
    {
        //
        // GET: /Rpt/OrdersByBrand/

        public ActionResult Index() //OrdersByBrand Rpt
        {
            return View();
        }
        public ActionResult OrderByBrand(Guid? brandId,DateTime? startDate,DateTime? endDate)
        {
            ViewBag.Parameters = "?brandId=" + brandId + "&startDate=" + startDate + "&endDate=" + endDate; 
            return View();
        }
        public ActionResult OrdersByDistributor(Guid? brandId,Guid? distributorId, DateTime? startDate, DateTime? endDate)
        {
            ViewBag.Parameters = "?brandId=" + brandId +"&distributorId="+distributorId+"&startDate=" + startDate + "&endDate=" + endDate; 

            return View();

        }
        public ActionResult OrdersBySalesmanPerBrand(Guid? brandId,Guid? distributorId,Guid?salesmanId, DateTime? startDate, DateTime? endDate)
        {
            ViewBag.Parameters = "?brandId=" + brandId + "&distributorId=" + distributorId +"&salesmanId=" +salesmanId+ "&startDate=" + startDate + "&endDate=" + endDate; 

            return View();
        }
        public  ActionResult OrdersDetails(Guid? orderId)
        {
            ViewBag.Parameters = "?id="+orderId;
            return View();
        }

    }
}
