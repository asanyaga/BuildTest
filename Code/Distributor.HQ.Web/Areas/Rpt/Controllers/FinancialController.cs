using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class FinancialController : Controller
    {
        //
        // GET: /Rpt/Financial/

        public ActionResult GrossProfitByDistributor()
        {
            return View();
        }

		public ActionResult SalesFinancialByTier()
		{
			return View();
		}
    }
}
