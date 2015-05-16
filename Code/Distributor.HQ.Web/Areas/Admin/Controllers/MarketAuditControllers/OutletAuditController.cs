using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.MarketAuditViewModelBuilders ;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.MarketAuditViewModels ;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.MarketAuditControllers
{
    [Authorize ]
    public class OutletAuditController : Controller
    {
        IOutletAuditViewModelBuilder _outletAuditViewModelBuilder;
        public OutletAuditController(IOutletAuditViewModelBuilder outletAuditViewModelBuilder)
        {
            _outletAuditViewModelBuilder = outletAuditViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListOutletAudits(bool? showInactive, int? page, int? itemsperpage)
        {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
            bool showinactive = false;
            if (showInactive != null)
                showinactive = (bool)showInactive;
            ViewBag.showInactive = showinactive;
            var ls = _outletAuditViewModelBuilder.GetAll(showinactive);
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            return View(ls.ToPagedList(currentPageIndex,  ViewModelBase.ItemsPerPage));

        }
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateOutletAudit()
        {
            return View("CreateOutletAudit", new OutletAuditViewModel());
        }
        public ActionResult OutletAuditDetails(Guid Id)
        {
            OutletAuditViewModel outletAudit = _outletAuditViewModelBuilder.Get(Id);
            return View(outletAudit);
        }
        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditOutletAudit(Guid Id)
        {
            OutletAuditViewModel outletAudit = _outletAuditViewModelBuilder.Get(Id);
            return View(outletAudit);
        }
        public ActionResult DeActivate(Guid Id)
        {
            try
            {
                _outletAuditViewModelBuilder.SetInactive(Id);
                TempData["msg"] = "Successfully Deactivated";

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListOutletAudits");
        }

        public JsonResult Owner(string blogName)
        {
            IList<OutletAuditViewModel> outletAudit = _outletAuditViewModelBuilder.GetAll(true);
            return Json(outletAudit);
        }
        [HttpPost]
        public ActionResult CreateOutletAudit(OutletAuditViewModel tpvm)
        {

            try
            {
                tpvm.Id = Guid.NewGuid();
                _outletAuditViewModelBuilder.Save(tpvm);
                return RedirectToAction("ListOutletAudits");
            }

            catch (DomainValidationException dve)
            {

                ValidationSummary.DomainValidationErrors(dve, ModelState);
                //ViewBag.msg = msg;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;

                return View();
            }
        }
        [HttpPost]
        public ActionResult EditOutletAudit(OutletAuditViewModel tpvm)
        {
            _outletAuditViewModelBuilder.Save(tpvm);
            return RedirectToAction("ListOutletAudits");
        }
        [HttpPost]
        public ActionResult ListOutletAudits(bool? showInactive, int? page, string srch, string distName, int? itemsperpage)
        {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
            string command = srch;
            bool showinactive = false;
            if (showInactive != null)
                showinactive = (bool)showInactive;
            ViewBag.showInactive = showinactive;
            var ls = _outletAuditViewModelBuilder.Search(distName, showinactive);
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            if (command == "Search")
            {
                return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
            }
            else
            {
                return RedirectToAction("ListOutletAudits", new { showinactive = showInactive, srch = "Search", distName = "" });
            }
        }
    }
}