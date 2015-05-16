using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class DistributorTargetsController : Controller
    {
        //
        // GET: /Rpt/DistributorTargets/

        public ActionResult Index(DateTime? startDate, DateTime? endDate,Guid? periodId)
        {
            ViewBag.Parameters = "?startDate=" + startDate + "&endDate=" + endDate+"&periodId"+periodId;
            return View();
        }

        public ActionResult DistributorTargetsByRegion(Guid countryId, DateTime? startDate, DateTime? endDate, Guid? periodId)
        {
            ViewBag.Parameters = "?countryId=" + countryId + "&startDate=" + startDate + "&endDate=" + endDate+"&periodId="+periodId;
            return View();
        }

        public ActionResult DistributorTargetsByDistributor(Guid countryId, Guid regionId, DateTime? startDate, DateTime? endDate, Guid? periodId)
        {
            ViewBag.Parameters = "?countryId=" + countryId +
                                 "&regionId=" + regionId +
                                 "&startDate=" + startDate +
                                 "&endDate=" + endDate+"&periodId="+periodId;
            return View();
        }
    }
}
