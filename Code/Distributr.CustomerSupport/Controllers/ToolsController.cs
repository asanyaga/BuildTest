using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.CustomerSupport.Code.Tools;

namespace Distributr.CustomerSupport.Controllers
{
    public class ToolsController : Controller
    {
        private readonly IToolViewModelBuilder _toolViewModelBuilder;

        public ToolsController(IToolViewModelBuilder toolViewModelBuilder)
        {
            _toolViewModelBuilder = toolViewModelBuilder;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Retire(Guid? docParentId = null)
        {
            if (docParentId == null || docParentId == Guid.Empty)
            {
                return Json(new {ok = false, data = "", message = "Enter a valid Guid"});
            }

            _toolViewModelBuilder.Retire(docParentId.Value);
            return Json(new {ok = true, data = new { archived = true}});
        }

        [HttpPost]
        public ActionResult UnRetire(Guid? docParentId = null)
        {
            if (docParentId == null || docParentId == Guid.Empty)
            {
                return Json(new { ok = false, data = "", message = "Enter a valid Guid" });
            } 

            _toolViewModelBuilder.UnRetire(docParentId.Value);
            return Json(new { ok = true, data = new { unarchived = true } });
        }
    }
}
