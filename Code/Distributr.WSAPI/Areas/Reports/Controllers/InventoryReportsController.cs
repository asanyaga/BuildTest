using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Reports.InventoryReports;

namespace Distributr.WSAPI.Areas.Reports.Controllers
{
    public class InventoryReportsController : Controller
    {
        private ICostCentreInventoryReportService _costCentreReportService;
       

        public InventoryReportsController(ICostCentreInventoryReportService costCentreReportService)
        {
            _costCentreReportService = costCentreReportService;
        }

        public ActionResult InventoryBalances(Guid? costCentreId)
        {
            var items = _costCentreReportService.GetInventoryByCostCentre(costCentreId,null );
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InventoryTransactions(Guid costCentreId, Guid productId)
        {
            var items = _costCentreReportService.GetInventoryTransactions(costCentreId, productId);
            return Json(items, JsonRequestBehavior.AllowGet);
        }

    }
}
