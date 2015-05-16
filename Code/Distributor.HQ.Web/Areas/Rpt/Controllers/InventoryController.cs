using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class InventoryController : Controller
    {
        //
        // GET: /Rpt/Inventory/
        //****NEW STOCK REPORTS**** 
        //STOCK SUMMARY REPORTS
        public ActionResult StockSummary2()
        {
            return View();
        }

        public ActionResult StockByBrand2(Guid? distributorId, Guid? productId, Guid? brandId)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&productId=" + productId + "&brandId=" + brandId;
            return View();
        }

        public ActionResult StockByProduct2(Guid? distributorId, Guid? brandId, Guid? productId)
        {
            ViewBag.Parameters = "?brandId=" + brandId + "&productId=" + productId + "&distributorId=" + distributorId;
            return View();
        }

        //STOCK BY SALESMAN SUMMARY REPORT
        //public ActionResult StockBySalesmanSummary()
        //{
        //    return View();
        //}
        //public ActionResult StockPerSalesman()
        //{
        //    return View();
        //}
        //public ActionResult StockBySalesmanPerBrand()
        //{
        //    return View();
        //}
        //public ActionResult StockBySalesmanPerProduct()
        //{
        //    return View();

        //}
        
        //STOCK BY DISTRIBUTOR

        public ActionResult StockByDistributor2()
        {
            return View();
        }
        public ActionResult StockByBrand3(Guid? distributorId)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId;
            return View();
        }
        public ActionResult StockByProduct3(Guid? distributorId, Guid? brandId)
        {
            ViewBag.Parameters = "?brandId=" + brandId + "&distributorId=" + distributorId;
            return View();
        }


        //*********END*************

        public ActionResult StockSummary()
        {
            return View();
        }

		public ActionResult StockByDistributor()
		{
			return View();
		}

		public ActionResult StockBySubBrand()
		{
			return View();
		}

		public ActionResult StockByPackaging()
		{
			return View();
		}

		public ActionResult StockByBrand()
		{
			return View();
		}

    	public ActionResult StockReturns()
		{
			return View();
		}

		public ActionResult StockReturnsSubReport(Guid distributorId)
		{
            ViewBag.Parameters = "?distributorId=" + distributorId;
			return View();
		}

    	public ActionResult StockSubReport(Guid id)
		{
			ViewBag.Parameters = "?id=" + id;
			return View();
		}

    	public ActionResult CloseOfDay()
		{
			return View();
		}

		public ActionResult ReorderLevel()
		{
			return View();
		}

		public ActionResult DataReport()
		{
			return View();
		}
        public ActionResult StockTakeReport() 
        {
            return View();
        }


        //STOCK BY SALESMAN SUMMARY REPORT
        public ActionResult StockBySalesmanSummary(Guid? distributorId, Guid? salesmanId)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&salesmanId=" + salesmanId;
            return View();
        }
        public ActionResult StockPerSalesman(Guid? distributorId, Guid? salesmanId)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&salesmanId=" + salesmanId;
            return View();
        }
        public ActionResult StockBySalesmanPerBrand(Guid? distributorId, Guid? salesmanId, Guid? brandId)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&salesmanId=" + salesmanId + "&brandId=" + brandId;
            return View();
        }
        public ActionResult StockBySalesmanPerProduct(Guid? distributorId, Guid? salesmanId, Guid? brandId, Guid? productId)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&salesmanId=" + salesmanId + "&brandId=" + brandId + "&productId=" + productId;
            return View();
        }
    }
}
