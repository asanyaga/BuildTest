using System;
using System.Web.Mvc;

namespace Agrimanagr.HQ.Areas.AgrimanagrRpt.Controllers
{
    public class ProducePurchaseController : Controller
    {
        //
        // GET: /AgrimanagrRpt/ProducePurchase/

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult ProducePurchaseByBuyingCenter(Guid? routeId, DateTime startDate, DateTime endDate)
        {
            ViewBag.Parameters = "?RouteId="+ routeId + "&startDate=" + startDate + "&andDate=" + endDate ;
            return View();
        }
        public ActionResult ProducePurchaseByFarmer(Guid? routeId, Guid? buyingCentreId, DateTime startDate, DateTime endDate)
        {
            ViewBag.Parameters = "?RouteId=" + routeId + "&BuyingCentreId=" + buyingCentreId + "&startDate=" + startDate + "&andDate=" + endDate;
            return View();
        }

        public ActionResult ProducePurchaseSummary(Guid? routeId, Guid? buyingCentreId,Guid? farmerId, DateTime startDate, DateTime endDate)
        {
            ViewBag.Parameters = "?RouteId=" + routeId + "&BuyingCentreId=" + buyingCentreId + "&farmerId=" + farmerId + "&startDate=" + startDate + "&andDate=" + endDate;
            return View();
        }
        public  ActionResult ProducePurchaseTransactionDetails(Guid? id)
        {
            ViewBag.Parameters = "?id=" + id;
            return View();
        }
    }
}
