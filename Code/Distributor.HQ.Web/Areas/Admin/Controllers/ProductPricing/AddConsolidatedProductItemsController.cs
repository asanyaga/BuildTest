using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Distributr.HQ.Web.Areas.Admin.Controllers
{
    [Authorize ]
    public class AddConsolidatedProductItemsController : Controller
    {
        //
        // GET: /Admin/AddConsolidatedProductItems/

        public ActionResult Index()
        {
            return View();
        }

    }
}
