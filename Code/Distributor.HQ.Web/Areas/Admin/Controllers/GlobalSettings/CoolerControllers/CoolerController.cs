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
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using System.Diagnostics;
using Distributr.Core.Utility.Validation;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings.CoolerControllers
{
     [Authorize]
    public class CoolerController : Controller
    { 
        ICoolerViewModelBuilder _coolerViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public CoolerController(ICoolerViewModelBuilder coolerViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _coolerViewModelBuilder = coolerViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
          [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListCoolers(bool? showInactive, int? page, int? itemsperpage)
        {
            Stopwatch stopWatch = new Stopwatch();
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
                stopWatch.Start();
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;
                ViewBag.showInactive = showinactive;
                var ls = _coolerViewModelBuilder.GetAll(showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                stopWatch.Stop();


                TimeSpan ts = stopWatch.Elapsed;


                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.TotalMilliseconds);


                stopWatch.Reset();
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "List Cooler Timer", "Cooler Controller" + elapsedTime, DateTime.Now);
                return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
               
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list cooler" + ex.Message);
                _log.Error("Failed to list cooler" + ex.ToString());
                return View();
            }
        }
          [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateCooler()
        {
            ViewBag.CoolerTypeList = _coolerViewModelBuilder.CoolerType();
            return View("CreateCooler", new CoolerViewModel());
        }
          public ActionResult CoolerDetails(Guid Id)
        {
            CoolerViewModel cooler = _coolerViewModelBuilder.Get(Id);
            return View(cooler);
        }
          [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditCooler(Guid Id)
        {
            try
            {
                ViewBag.CoolerTypeList = _coolerViewModelBuilder.CoolerType();
                CoolerViewModel cooler = _coolerViewModelBuilder.Get(Id);
                return View(cooler);
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to edit cooler" + ex.Message);
                _log.Error("Failed to edit cooler" + ex.ToString());
                return View();
            }
        }
          public ActionResult DeActivate(Guid Id)
        {
            try
            {
                _coolerViewModelBuilder.SetInactive(Id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Cooler", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate cooler" + ex.Message);
                _log.Error("Failed to deactivate cooler" + ex.ToString());
            }
            return RedirectToAction("ListCoolers");
        }

        public JsonResult Owner(string blogName)
        {
            IList<CoolerViewModel> cooler = _coolerViewModelBuilder.GetAll(true);
            return Json(cooler);
        }
        [HttpPost]
        public ActionResult CreateCooler(CoolerViewModel ctpvm)
        {
            ViewBag.CoolerTypeList = _coolerViewModelBuilder.CoolerType();
            try
            {
                ctpvm.Id = Guid.NewGuid();
                _coolerViewModelBuilder.Save(ctpvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Cooler", DateTime.Now);
                TempData["msg"] = "Cooler Successfully Created";
                return RedirectToAction("ListCoolers");
            }

            catch (DomainValidationException dve)
            {

                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to create cooler" + dve.Message);
                _log.Error("Failed to create cooler" + dve.ToString());
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                _log.Debug("Failed to create cooler" + ex.Message);
                _log.Error("Failed to create cooler" + ex.ToString());
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditCooler(CoolerViewModel ctpvm)
        {
            try
            {
                ViewBag.CoolerTypeList = _coolerViewModelBuilder.CoolerType();

                _coolerViewModelBuilder.Save(ctpvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Cooler", DateTime.Now);
                TempData["msg"] = "Cooler Successfully Edited";
                return RedirectToAction("ListCoolers");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                return View();
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to edit cooler" + ex.Message);
                _log.Error("Failed to edit cooler" + ex.ToString());
                return View();
            }
        }
        [HttpPost]
        public ActionResult ListCoolers(bool? showInactive, int? page, string srch, string distName, int? itemsperpage)
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
                var ls = _coolerViewModelBuilder.Search(distName, showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                if (command == "Search")
                {
                    return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
                }
                else
                {
                    return RedirectToAction("ListCoolers", new { showinactive = showInactive, srch = "Search", distName = "" });
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
