using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CoolerViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.CoolerViewModel;
using log4net;
using System.Reflection;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings.CoolerControllers
{
     [Authorize]
    public class CoolerTypeController : Controller
    { 
        ICoolerTypeViewModelBuilder _coolerTypeViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public CoolerTypeController(ICoolerTypeViewModelBuilder coolerTypeViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _coolerTypeViewModelBuilder = coolerTypeViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListCoolerTypes(bool? showInactive, int? page, int? itemsperpage)
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;
                ViewBag.showInactive = showinactive;
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                var ls = _coolerTypeViewModelBuilder.GetAll(showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                return View(ls.ToPagedList(currentPageIndex,  ViewModelBase.ItemsPerPage));
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list cooler type" + ex.Message);
                _log.Error("Failed to list cooler type" + ex.ToString());
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateCoolerType()
        {
            return View("CreateCoolerType", new CoolerTypeViewModel());
        }
        public ActionResult CoolerTypeDetails(Guid Id)
        {
            CoolerTypeViewModel coolerType = _coolerTypeViewModelBuilder.Get(Id);
            return View(coolerType);
        }
        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditCoolerType(Guid Id)
        {
            try
            {
                CoolerTypeViewModel coolerType = _coolerTypeViewModelBuilder.Get(Id);
                return View(coolerType);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        public ActionResult DeActivate(Guid Id)
        {
            try
            {
                _coolerTypeViewModelBuilder.SetInactive(Id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Cooler Type", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";

            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                _log.Debug("Failed to deactivate cooler type" + dve.Message);
                _log.Error("Failed to deactivate cooler type" + dve.ToString());
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate cooler type" + ex.Message);
                _log.Error("Failed to deactivate cooler type" + ex.ToString());
            }
            return RedirectToAction("ListCoolerTypes");
        }

        public JsonResult Owner(string blogName)
        {
            IList<CoolerTypeViewModel> coolerType = _coolerTypeViewModelBuilder.GetAll(true);
            return Json(coolerType);
        }
        [HttpPost]
        public ActionResult CreateCoolerType(CoolerTypeViewModel ctpvm)
        {

            try
            {
                ctpvm.Id = Guid.NewGuid();
                _coolerTypeViewModelBuilder.Save(ctpvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Cooler Type", DateTime.Now);
                return RedirectToAction("ListCoolerTypes");
            }

            catch (DomainValidationException dve)
            {

                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to create cooler type" + dve.Message);
                _log.Error("Failed to create cooler type" + dve.ToString());
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;

                return View();
            }
        }
        [HttpPost]
        public ActionResult EditCoolerType(CoolerTypeViewModel ctpvm)
        {
            try
            {
                _coolerTypeViewModelBuilder.Save(ctpvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Cooler Type", DateTime.Now);
                return RedirectToAction("ListCoolerTypes");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                return View();
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to edit cooler type" + ex.Message);
                _log.Error("Failed to edit cooler type" + ex.ToString());
                return View();
            }
        }
        [HttpPost]
        public ActionResult ListCoolerTypes(bool? showInactive, int? page, string srch, string distName, int? itemsperpage)
        {
            try
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
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                var ls = _coolerTypeViewModelBuilder.Search(distName, showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                if (command == "Search")
                {
                    return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
                }
                else
                {
                    return RedirectToAction("ListCoolerTypes", new { showinactive = showInactive, srch = "Search", distName = "" });
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
    }
}
