using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class SalesByRetailerController : Controller
    {
        //
        // GET: /Rpt/SalesByRetailer/

        public ActionResult Index(Guid? distributorId,DateTime? startDate,DateTime? endDate,Guid? salesmanId,Guid? routeId)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&startDate=" + startDate + "&endDate=" + endDate+"&salesmanId="+salesmanId+"&routeId="+routeId;

            return View();
        }

    }
}
