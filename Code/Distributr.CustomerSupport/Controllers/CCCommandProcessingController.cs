using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.CustomerSupport.Code.CCCommandProcessing;
using Distributr.WSAPI.Lib.Services.Routing;

namespace Distributr.CustomerSupport.Controllers
{
    public class CCCommandProcessingController : Controller
    {
        private ICCCommandProcessingViewModelBuilder _ccCommandProcessingViewModelBuilder;
        private ICommandEnvelopeRouteOnRequestCostcentreRepository _envelopeRouteOnRequestCostcentreRepository;
        public CCCommandProcessingController(ICCCommandProcessingViewModelBuilder ccCommandProcessingViewModelBuilder, ICommandEnvelopeRouteOnRequestCostcentreRepository envelopeRouteOnRequestCostcentreRepository)
        {
            _ccCommandProcessingViewModelBuilder = ccCommandProcessingViewModelBuilder;
            _envelopeRouteOnRequestCostcentreRepository = envelopeRouteOnRequestCostcentreRepository;
        }

        public ActionResult Index()
        {
            var vm =  _ccCommandProcessingViewModelBuilder.GetCommandProcessingSummary();
            return View(vm);
        }
        public ActionResult Update()
        {
            var start = DateTime.Now;
           _envelopeRouteOnRequestCostcentreRepository.UpdateStatus();

          
            var end = DateTime.Now.Subtract(start);
            ViewBag.Status = string.Format("Success Start End Diff {0} ", end);
            return View();
        }
        public ActionResult ReQueue(string post)
        {
            if (!string.IsNullOrEmpty(post))
            {
                _ccCommandProcessingViewModelBuilder.QueueCommands();
            }
            ViewBag.count = _ccCommandProcessingViewModelBuilder.GetUnQueuedCommands();
           
            return View();
        }
        public ActionResult Test()
        {
           _ccCommandProcessingViewModelBuilder.Test();
            return View();
        }
      
        public ActionResult Detail(Guid costCentreId)
        {
            DateTime dt = DateTime.Now;
            var vm = _ccCommandProcessingViewModelBuilder.GetCommandDetail(dt.DayOfYear, dt.Year, costCentreId);
            return View(vm);
        }

        [HttpPost]
        public ActionResult Detail(Guid costCentreId, string shortdate)
        {
            DateTime dt = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(shortdate))
                DateTime.TryParse(shortdate, out dt);
            var vm = _ccCommandProcessingViewModelBuilder.GetCommandDetail(dt.DayOfYear, dt.Year, costCentreId);
            return View(vm);
        }

    }
}
