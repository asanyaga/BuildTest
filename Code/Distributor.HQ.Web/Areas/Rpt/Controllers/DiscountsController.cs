using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class DiscountsController : Controller
    {
        //
        // GET: /Rpt/Discounts/
        #region New Discount Reports
        public  ActionResult TotalDiscountsByDistributor()
        {
            return View();
        }
        public ActionResult DiscountByDistributorPerProduct(Guid distributorId, int discounType, DateTime startDate, DateTime endDate)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&discounType=" + discounType + "&startDate=" + startDate + "&endDate=" + endDate;
            return View();
        }
        #endregion



        public ActionResult FreeOfChargeByProduct()
        {
            return View();
        }

		public ActionResult FreeOfChargeByDistributor()
		{
			return View();
		}

		public ActionResult PromotionDiscountByDistributor()
		{
			return View();
		}

		public ActionResult PromotionDiscountByProduct()
		{
			return View();
		}

		public ActionResult CertainValueCertainProduct()
		{
			return View();
		}

    }
}
