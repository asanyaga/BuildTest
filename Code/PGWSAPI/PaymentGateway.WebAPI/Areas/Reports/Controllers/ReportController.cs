using System.Web.Mvc;

namespace PaymentGateway.WebAPI.Areas.Reports.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Reports/Report/

        public ActionResult ServiceProvider()
        {
            return View();
        }

    }
}
