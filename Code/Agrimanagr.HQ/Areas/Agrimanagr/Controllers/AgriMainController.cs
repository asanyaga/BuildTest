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
            return View();
        }

    }
}
