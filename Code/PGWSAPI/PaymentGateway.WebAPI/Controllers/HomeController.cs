using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace PaymentGateway.WebAPI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult About()
        {
            string version = "Payment Gateway Bridge version: " +
                             ParseVersionNumber(Assembly.GetExecutingAssembly()).ToString();

            return Json(version, JsonRequestBehavior.AllowGet);
        }

        private static Version ParseVersionNumber(Assembly assembly)
        {
            AssemblyName assemblyName = new AssemblyName(assembly.FullName);
            return assemblyName.Version;
        }
    }
}
