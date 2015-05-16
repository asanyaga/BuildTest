using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class OutletsController : Controller
    {
        //
        // GET: /Rpt/Outlets/

        public ActionResult DeactivatedOutlets()
        {
            return View();
        }

		public ActionResult NewOutlets()
		{
			return View();
		}

		public ActionResult OutletsByCategory()
		{
			return View();
		}

		public ActionResult OutletsByChannel()
		{
			return View();
		}

		public ActionResult OutletsByDistributor()
		{
			return View();
		}
       //
        public ActionResult OutletsByDistributor2()
        {
            return View();
        }
	  //	
        public ActionResult OutletsByTier()
		{
			return View();
		}

        public ActionResult Routes() 
        {
            return View();
        }
        public ActionResult Outlets()
        {
            return View();
        }
    }
}
