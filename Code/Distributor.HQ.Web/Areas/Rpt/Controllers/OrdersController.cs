using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class OrdersController : Controller
    {
        //
        // GET: /Rpt/Orders/

        public ActionResult OrdersSummary()
		{
			return View();
		}

		public ActionResult OrdersSubReport(Guid id)
		{
			ViewBag.Parameters = "?id=" + id;
			return View();
		}

    	public ActionResult OrdersByDistributor()
		{
			return View();
		}

		public ActionResult OrdersByBrand()
		{
			return View();
		}

		public ActionResult OrdersBySubBrand()
		{
			return View();
		}

		public ActionResult OrdersByProduct()
		{
			return View();
		}

		public ActionResult OrdersByPackaging()
		{
			return View();
		}

		public ActionResult OrderDateException()
		{
			return View();
		}
    }
}
