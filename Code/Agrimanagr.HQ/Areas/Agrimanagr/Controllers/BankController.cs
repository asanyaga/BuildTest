using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.BankViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using log4net;
using System.Reflection;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModels.Admin.BankViewModels;
using Distributr.HQ.Lib.Validation;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers
{
    /*[Authorize ]*/
    //
    public class BankController : Controller
    {
        IBankViewModelBuilder _bankViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public BankController(IBankViewModelBuilder bankViewModelBuilder)
        {
            _bankViewModelBuilder = bankViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
       [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListBanks(bool? showInactive, string srchParam = "", int page = 1, int itemsperpage = 10)
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;
                ViewBag.showInactive = showinactive;
                ViewBag.SearchText = srchParam;
                var currentPageIndex = page-1 < 0 ? 0 : page-1;
                var take = itemsperpage;
                var skip = currentPageIndex * take;
                var query = new QueryStandard { Name = srchParam, ShowInactive = showinactive, Skip = skip, Take = take };
                var ls = _bankViewModelBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data.ToList();
                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, total));
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                return View();
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list bank" + ex.Message);
                _log.Error("Failed to list bank" + ex.ToString());
                return View();
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateBank()
        {
            
            return View("CreateBank", new BankViewModel());
        }
        public ActionResult BankDetails(Guid Id)
        {
            try
            {
                BankViewModel bank = _bankViewModelBuilder.Get(Id);
                return View(bank);
            }
            catch (Exception exx)
            { 
            ViewBag.msg=exx.Message;
            return View();
            }
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult EditBank(Guid Id)
        {
            try
            {
                BankViewModel bank = _bankViewModelBuilder.Get(Id);
                return View(bank);
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to edit bank" + ex.Message);
                _log.Error("Failed to edit bank" + ex.ToString());
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        public ActionResult DeActivate(Guid Id)
        {
            try
            {
                _bankViewModelBuilder.SetInactive(Id);
                TempData["msg"] = "Successfully Deactivated";

            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate bank" + ex.Message);
                _log.Error("Failed to deactivate bank" + ex.ToString());
            }
            return RedirectToAction("ListBanks");
        }

        public JsonResult Owner(string blogName)
        {
            IList<BankViewModel> bank = _bankViewModelBuilder.GetAll(true);
            return Json(bank);
        }
        [HttpPost]
        public ActionResult CreateBank(BankViewModel ctpvm)
        {
           
            try
            {
                ctpvm.Id = Guid.NewGuid();
                _bankViewModelBuilder.Save(ctpvm);
                TempData["msg"] = "Bank Successfully Created";
                return RedirectToAction("ListBanks");
            }

            catch (DomainValidationException dve)
            {

                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to create bank" + dve.Message);
                _log.Error("Failed to create bank" + dve.ToString());
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                _log.Debug("Failed to create bank" + ex.Message);
                _log.Error("Failed to create bank" + ex.ToString());
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditBank(BankViewModel ctpvm)
        {
            try
            {

                _bankViewModelBuilder.Save(ctpvm);
                TempData["msg"] = "Bank Successfully Edited";
                return RedirectToAction("ListBanks");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                return View();
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to edit bank" + ex.Message);
                _log.Error("Failed to edit bank" + ex.ToString());
                return View();
            }
        }
        
        public ActionResult Activate(Guid Id)
        {
            try
            {
                _bankViewModelBuilder.SetActive(Id);
                TempData["msg"] = "Successfully activated";

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate bank" + ex.Message);
                _log.Error("Failed to activate bank" + ex.ToString());
            }
            return RedirectToAction("ListBanks");
        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult Delete(Guid Id)
        {
            try
            {
                _bankViewModelBuilder.SetAsDeleted(Id);
                TempData["msg"] = "Successfully deleted";

            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to delete bank" + ex.Message);
                _log.Error("Failed to delete bank" + ex.ToString());
            }
            return RedirectToAction("ListBanks");
        }

    
    }
}
