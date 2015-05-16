using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Distributr.CustomerSupport.Code.CCRouting;
using System.Web.Http;

namespace Distributr.CustomerSupport.Controllers
{
    public class CCRoutingController : Controller
    {
        private ICCRoutingVMBuilder _ccRoutingVmBuilder;

        public CCRoutingController(ICCRoutingVMBuilder ccRoutingVmBuilder)
        {
            _ccRoutingVmBuilder = ccRoutingVmBuilder;
        }

        public ActionResult Index()
        {
            var vm = _ccRoutingVmBuilder.GetRoutingSummary();
            return View(vm);
        }

        public ActionResult Detail(Guid costCentreId)
        {
            DateTime dt = DateTime.Now;
            var vm = _ccRoutingVmBuilder.RoutingDetail(costCentreId, dt.DayOfYear, dt.Year);
            return View(vm);
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Detail(Guid costCentreId, string shortDate)
        {
            DateTime dt = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(shortDate))
                DateTime.TryParse(shortDate, out dt);
            var vm = _ccRoutingVmBuilder.RoutingDetail(costCentreId, dt.DayOfYear, dt.Year);
            return View(vm);
        }

    }
}
