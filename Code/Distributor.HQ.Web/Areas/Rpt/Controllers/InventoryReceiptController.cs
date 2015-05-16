using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class InventoryReceiptController : Controller
    {
        //
        // GET: /Rpt/InventoryReceipt/

        public ActionResult InventoryReceiptByBrand()
        {
            return View();
        }

		public ActionResult InventoryReceiptByDistributor()
		{
			return View();
		}

		public ActionResult InventoryReceiptBySubBrand()
		{
			return View();
		}

		public ActionResult InventoryReceiptByPackaging()
		{
			return View();
		}
    }
}
