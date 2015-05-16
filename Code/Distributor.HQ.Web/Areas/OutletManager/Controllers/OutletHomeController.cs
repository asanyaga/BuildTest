using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.OutletManager.Controllers
{
    public class OutletHomeController : Controller
    {
        //
        // GET: /OutletManager/OuteletHome/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

    }
}
