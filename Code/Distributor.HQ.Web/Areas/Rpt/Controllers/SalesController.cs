using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class SalesController : Controller
    {
        #region REPORT SERVER SALES REPORTS
        #region SaleSummary
        public ActionResult RsSaleSummaryByDate()
        {
            return View();
        }
        #endregion
        #region Close Of Day
        public ActionResult RsCloseOfDay()
        {
            return View();
        }
        #endregion
        #region Losses
        public ActionResult RsLosses()
        {
            return View();
        }
        #endregion
        public ActionResult Reports (string reportName)
        {
            ViewBag.ReportName = reportName;
            return View();
        }
        #endregion

        //
        // GET: /Rpt/Sales/
        //SALE SUMMARY REPORTS
        public ActionResult SaleSummary() 
        {
            return View();
        }
        
        public ActionResult SaleByDistributor2(Guid? distributorId, DateTime? startDate, DateTime? endDate) 
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&startDate=" + startDate + "&endDate=" + endDate;
            return View();
        }

        public ActionResult SaleBySalesman(Guid? distributorId,Guid? salesmanId, DateTime? startDate, DateTime? endDate) 
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&salesmanId=" + salesmanId + "&startDate=" + startDate + "&endDate=" +
          endDate;
            return View();
        }
        public ActionResult SaleBySalesmanPerBrand(Guid? distributorId, Guid? salesmanId,Guid? brandId, DateTime? startDate, DateTime? endDate) 
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&salesmanId=" + salesmanId + "&brandId=" + brandId + "&startDate=" + startDate + "&endDate=" +
         endDate;
            return View();
        }

        public ActionResult SaleDetails() 
        {
            return View();
        }
        //END SALE SUMMARY RPTS
        //SALE BY DISTRIBUTOR RPTS
        public ActionResult SaleByDistributor()
        {
            return View();
        }
        public ActionResult SalesBySalesman(Guid distributorId, DateTime startDate,DateTime endDate)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&startDate=" + startDate + "&endDate=" + endDate;
            return View();
        }
        public ActionResult SaleSummary2(Guid distributorId,Guid salesmanId, Guid routeId, Guid outletId, DateTime startDate, DateTime endDate)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&salesmanId=" + salesmanId + "&routeId=" + routeId + "&outletId=" + outletId + "&startDate=" + startDate + "&endDate=" + endDate;
            return View();
        }
        public ActionResult SaleBySalesmanPerBrand2(Guid distributorId, Guid salesmanId,Guid brandId, DateTime startDate, DateTime endDate)
        {
            ViewBag.Parameters = "?distributorId="+distributorId+"&salesmanId="+salesmanId+"&brandId="+brandId+"&startDate="+startDate+"&endDate="+endDate;
                ; return View();
        }
        public ActionResult SalesDetails()
        {
            return View();
        }

        //**END**


        public ActionResult SalesSummary()
        {
            return View();
        }

        public ActionResult SalesSummary2(Guid distributorId, Guid salesmanId, Guid routeId, Guid outletId,  DateTime startDate, DateTime endDate)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&salesmanId=" + salesmanId + "&routeId=" + routeId + "&outletId=" + outletId +  "&startDate=" + startDate + "&endDate=" + endDate;           
            return View();
        }

		public ActionResult SalesSubReport(Guid id)
		{
			ViewBag.Parameters = "?id=" + id;
			return View();
		}
        
    	public ActionResult SalesByBrand()
    	{
			return View();
		}

        public ActionResult SaleByBrand(Guid? brandId, DateTime startDate, DateTime endDate)
        {
            ViewBag.Parameters = "?brandid=" + brandId + "&startDate=" + startDate + "&endDate=" + endDate;           
            //ViewBag.Parameters = "brandid=" + brandId + "&startDate=" + startDate + "&endDate=" + endDate;           

            return View();
        }

		public ActionResult SalesByDistributor(Guid brandId,Guid distributorId, DateTime startDate, DateTime endDate)
		{
            ViewBag.Parameters = "?brandId=" + brandId + "&distributorId=" +distributorId + "&startDate=" + startDate + "&endDate=" + endDate;
			return View();
		}

        public ActionResult SalesBySubBrand()
		{
			return View();
		}

		public ActionResult SalesByProduct()
		{
			return View();
		}

		public ActionResult SalesByPackaging()
		{
			return View();
		}

    	public ActionResult SalesDateException()
		{
			return View();
		}

        public ActionResult ZeroSalesReport()
        {
            return View();
        }

		public ActionResult PaymentSummary()
		{
			return View();
		}

		public ActionResult PaymentSubReport(Guid outlet, Guid distributor, Guid salesman, DateTime startDate, DateTime endDate, int paymentMode)
		{
			ViewBag.Parameters =
				"?outlet=" + outlet + "&distributor="+distributor + "&salesman=" + salesman + "&startDate=" + startDate + "&endDate=" + endDate + 
				"&paymentMode=" + paymentMode;
			return View();
		}

        public ActionResult SalesBySalesmanPerBrand(Guid salesmanId, Guid distributorId,Guid brandId, DateTime startDate, DateTime endDate)
        {
            ViewBag.Parameters ="?salesmanId=" + salesmanId + "&distributorId=" + distributorId +"&brandId="+brandId+ "&startDate=" + startDate + "&endDate=" + endDate;
            return View();
        }
    }
}
