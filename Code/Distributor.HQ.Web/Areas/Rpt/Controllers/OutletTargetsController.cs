using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class OutletTargetsController : Controller
    {
        //
        // GET: /Rpt/OutletTargets/

        public ActionResult Index( DateTime? startDate, DateTime? endDate ,Guid? periodId)
        {
            ViewBag.Parameters = "?startDate=" + startDate + "&endDate=" + endDate + "&periodId=" + periodId ;
            return View();
        }

        public ActionResult OutletTargetsByRegion( Guid countryId, DateTime? startDate, DateTime? endDate, Guid periodId)
        {
            ViewBag.Parameters = "?countryId=" + countryId + "&startDate=" + startDate + "&endDate=" + endDate + "&periodId=" + periodId;
            return View();
        }

        public ActionResult OutletTargetsByDistributor( Guid countryId, Guid regionId, DateTime? startDate, DateTime? endDate, Guid? periodId)
        {
            ViewBag.Parameters =
                "?countryId=" + countryId +
                "&regionId=" + regionId +
                "&startDate=" + startDate +
                "&endDate=" + endDate +
                "&periodId=" + periodId;
            return View();
        }

        public ActionResult OutletTargetsByRoute(Guid countryId, Guid regionId, Guid distributorId,  DateTime? startDate, DateTime? endDate, Guid? periodId)
        {
            ViewBag.Parameters = "?countryId=" + countryId +
                                 "&regionId=" + regionId +
                                 "&distributorId=" + distributorId +
                                 "&startDate=" + startDate +
                                 "&endDate=" + endDate +
                                 "&periodId=" + periodId;
                                 
            return View();
        }

        public ActionResult OutletTargetsByOutlet(Guid countryId, Guid regionId, Guid distributorId, Guid routeId, DateTime? startDate, DateTime? endDate,  Guid? periodId)
        {
            ViewBag.Parameters = "?countryId=" + countryId +
                                 "&regionId=" + regionId +
                                 "&distributorId=" + distributorId +
                                 "&routeId=" + routeId +
                                 "&startDate=" + startDate +
                                 "&endDate=" + endDate+
                                 "&periodId=" + periodId;
            return View();
        }

    }
}
