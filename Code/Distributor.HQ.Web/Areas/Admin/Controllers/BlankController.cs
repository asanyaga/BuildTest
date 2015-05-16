using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Admin.Controllers
{
    [Authorize ]
    public class BlankController : Controller
    {
        //
        // GET: /Admin/Blank/WTF??

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Blank()
        {
            string msg = "Blank";
            return View(msg);
        }
    }
}
