using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class SalesSummaryController : Controller
    {
        //
        // GET: /Rpt/SalesSummary/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult SaleByDistributor(Guid? distributorId,DateTime startDate,DateTime endDate)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&startDate=" + startDate + "&endDate=" + endDate;
            return View();
        }


        public ActionResult SaleBySalesman(Guid? distributorId,Guid? salesmanId,DateTime startDate,DateTime endDate)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&salesmanId=" + salesmanId + "&startDate=" + startDate + "&endDate=" + endDate;
            return View();
        }


        public ActionResult SaleBySalesmanPerBrand(Guid? distributorId, Guid? salesmanId, Guid? brandId, DateTime startDate, DateTime endDate)
        {
            ViewBag.Parameters = "?distributorId=" + distributorId + "&salesmanId=" + salesmanId + "&brandId=" + brandId + "&startDate=" + startDate + "&endDate=" + endDate;
            return View();
        }

        public ActionResult SaleDetails()
        {
            return View();
        }
     

    }
}
