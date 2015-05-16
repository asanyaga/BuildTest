using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.System
{
    [Authorize ]
    public class SecurityMatrixController : Controller
    {
        //
        // GET: /Admin/SecurityMatrix/

        public ActionResult Index()
        {
            return View();
        }

    }
}
