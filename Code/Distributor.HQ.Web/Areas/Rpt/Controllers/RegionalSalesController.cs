using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Rpt.Controllers
{
    public class RegionalSalesController : Controller
    {
        //
        // GET: /Rpt/RegionalSales/

        public ActionResult Index()
        {
          
            return View();
        }


        public ActionResult SaleByCountry(Guid? countryId, DateTime startDate, DateTime endDate)
        {
            ViewBag.Parameters = "?CountryId=" + countryId + "&startDate=" + startDate + "&endDate=" + endDate;
            return View(); 
        }


        public ActionResult SaleByRegion(Guid? countryId, Guid? regionId, DateTime startDate, DateTime endDate)
        {
            ViewBag.Parameters = "?CountryId=" + countryId + "&regionId=" + regionId + "&startDate=" + startDate + "&endDate=" + endDate;
            return View();
        }

       
        public ActionResult SaleByDistributor(Guid? countryId, Guid? regionId, Guid? distributorId, DateTime startDate, DateTime endDate)
        {
            ViewBag.Parameters = "?CountryId=" + countryId + "&regionId=" + regionId + "&distributorId=" + distributorId  + "&startDate=" + startDate + "&endDate=" + endDate;
            return View();
        }


        public ActionResult SaleByRoute(Guid? countryId, Guid? regionId, Guid? distributorId, Guid? routeId, DateTime startDate, DateTime endDate)
        {
            ViewBag.Parameters = "?CountryId=" + countryId + "&regionId=" + regionId + "&distributorId=" + distributorId + "&routeId=" + routeId + "&startDate=" + startDate + "&endDate=" + endDate;
            return View();
        }


        public ActionResult SaleBySalesmanPerBrand(Guid? countryId, Guid? regionId, Guid? distributorId, Guid? routeId, Guid? outletId, DateTime startDate, DateTime endDate)
        {
            ViewBag.Parameters = "?CountryId=" + countryId + "&regionId=" + regionId + "&distributorId=" + distributorId + "&routeId=" + routeId + "&outletId=" + outletId +  "&startDate=" + startDate + "&endDate=" + endDate;

            return View();
        }



    }
}
