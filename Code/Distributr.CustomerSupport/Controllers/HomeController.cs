using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.CustomerSupport.Code.Summary;

namespace Distributr.CustomerSupport.Controllers
{
    public class HomeController : Controller
    {
        private ISummaryVMBuilder _summaryVmBuilder;

        public HomeController(ISummaryVMBuilder summaryVmBuilder)
        {
            _summaryVmBuilder = summaryVmBuilder;
        }

        public ActionResult Index()
        {
           // var vm = _summaryVmBuilder.HitSummary();
            return View();
        }

    }
}
