using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.MongoDB.Repository;
using Distributr.CustomerSupport.Code.CCAudit;

namespace Distributr.CustomerSupport.Controllers
{
    public class CCAuditLogController : Controller
    {
        private ICCAuditViewModelBuilder _auditViewModelBuilder;

        public CCAuditLogController(ICCAuditViewModelBuilder auditViewModelBuilder)
        {
            _auditViewModelBuilder = auditViewModelBuilder;
        }


        public ActionResult Index()
        {
            var dt = DateTime.Now;
            var vm = _auditViewModelBuilder.GetSummary(dt.DayOfYear,dt.Year);
            
            return View(vm);
        }
        [HttpPost]
        public ActionResult Index(string date)
        {
            DateTime dt = DateTime.Now;
            bool ok = DateTime.TryParse(date, out dt);
            var vm = _auditViewModelBuilder.GetSummary(dt.DayOfYear, dt.Year);
            return View(vm);
        }

        public ActionResult HitSummary(string date, string costCentre, string selectedAction="")
        {
            DateTime dt = DateTime.Now;
            DateTime.TryParse(date, out dt);
            var vm = _auditViewModelBuilder.GetCostCentreHitSummary(dt.DayOfYear, dt.Year, new Guid(costCentre), selectedAction);

            return View(vm);
        }

    }
}
