using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PaymentGateway.WSApi.Areas.Reports.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Reports/Report/

        public ActionResult ServiceProvider()
        {
            return View();
        }

    }
}
