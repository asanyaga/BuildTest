using System.Web.Mvc;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers
{
    public class AgriMainController : Controller
    {
        //
        // GET: /Agrimanagr/AgrMain/



        public ActionResult Index()
        {
            ViewBag.logged = this.User.Identity.Name.ToString();
            //return View();
            //<li>@Html.ActionLink("DashBoard", "ReportView", new { reportName = "DashBoard", reporturi = "/Agrimanagr.Reports/DB_Agrimanagr_Main" })</li>
                
            return RedirectToAction("ReportView","ReportsDashboard", new { reportName = "DashBoard", reporturi = "/Agrimanagr.Reports/DB_Agrimanagr_Main" });
        }

    }
}
