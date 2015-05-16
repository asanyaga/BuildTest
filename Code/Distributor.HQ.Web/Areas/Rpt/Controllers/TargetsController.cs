using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class TargetsController : Controller
    {
        #region Older Reports

        //
        // GET: /Rpt/Targets/
        public ActionResult DistributorTargets()
        {
            return View();
        }

        public ActionResult OutletTargets()
        {
            return View();
        }

        #endregion

        public ActionResult Index(DateTime? startDate,DateTime? endDate)
        {
            ViewBag.Parameters = "?startDate=" + startDate + "&endDate=" + endDate;
            return View();
        }

        public ActionResult TargetsByRegion(Guid countryId, DateTime? startDate,DateTime? endDate)
        {
            ViewBag.Parameters = "?countryId=" + countryId + "&startDate=" + startDate + "&endDate=" + endDate;
            return View();
        }

        public ActionResult TargetsByDistributor(Guid countryId, Guid regionId, DateTime? startDate,DateTime? endDate)
        {
            ViewBag.Parameters = "?countryId=" + countryId +
                                 "&regionId=" + regionId + 
                                 "&startDate=" + startDate + 
                                 "&endDate=" + endDate; 
            return View();
        }

        public ActionResult TargetsByRoute(Guid countryId, Guid regionId, Guid distributorId, DateTime? startDate,DateTime? endDate)
        {
            ViewBag.Parameters = "?countryId=" + countryId +
                                 "&regionId=" + regionId +
                                 "&distributorId=" + distributorId +
                                 "&startDate=" + startDate + 
                                 "&endDate=" + endDate; 
            return View();
        }

        public ActionResult TargetsByOutlet(Guid countryId, Guid regionId, Guid distributorId, Guid routeId, DateTime? startDate,DateTime? endDate)
        {
            ViewBag.Parameters = "?countryId=" + countryId +
                                 "&regionId=" + regionId +
                                 "&distributorId=" + distributorId +
                                 "&routeId=" + routeId + 
                                 "&startDate=" + startDate + 
                                 "&endDate=" + endDate; 
            return View();
        }
    }
}
