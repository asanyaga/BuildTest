using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

namespace Distributr.HQ.Web.Areas.Admin.Controllers.BankControllers
{
    [Authorize]
    public class BankBranchController : Controller
    { 
        IBankBranchViewModelBuilder _bankBranchViewModelBuilder;
        IBankViewModelBuilder _bankViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public BankBranchController(IBankBranchViewModelBuilder bankBranchViewModelBuilder,IBankViewModelBuilder bankViewModelBuilder)
        {
            _bankBranchViewModelBuilder = bankBranchViewModelBuilder;
            _bankViewModelBuilder = bankViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListBankBranches(bool? showInactive, string searchText, int page = 1, int itemsperpage = 10)
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
                ViewBag.searchParam = searchText;

                int currentPageIndex = page < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex*take;

                var query = new QueryStandard(){ ShowInactive = showinactive, Skip = skip, Take = take,Name = searchText};

                var results = _bankBranchViewModelBuilder.Query(query);
                var count = results.Count;
                var data = results.Data.ToList();

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                return View();
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list bankBranch" + ex.Message);
                _log.Error("Failed to list bankBranch" + ex.ToString());
                
               
                ViewBag.msg = ex.Message;
              
                return View();
            }
        }
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateBankBranch()
        {
            ViewBag.BankList = _bankBranchViewModelBuilder.Bank();
            return View("CreateBankBranch", new BankBranchViewModel());
        }
        public ActionResult BankBranchDetails(Guid Id)
        {
            try
            {
                BankBranchViewModel bankBranch = _bankBranchViewModelBuilder.Get(Id);
                return View(bankBranch);
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                
                return View();
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
                
                return View();
            }
        }
        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditBankBranch(Guid Id)
        {
            try
            {
                ViewBag.BankList = _bankBranchViewModelBuilder.Bank();

                BankBranchViewModel bankBranch = _bankBranchViewModelBuilder.Get(Id);
                return View(bankBranch);
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count() > 0)
                    .Select(x => new { x.Key, x.Value.Errors })
                    .ToArray();
                return View();
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to edit bankBranch" + ex.Message);
                _log.Error("Failed to edit bankBranch" + ex.ToString());
               
               
                return View();
            }
        }
        public ActionResult DeActivate(Guid Id)
        {
            try
            {
                _bankBranchViewModelBuilder.SetInactive(Id);
                TempData["msg"] = "Successfully Deactivated";

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate bankBranch" + ex.Message);
                _log.Error("Failed to deactivate bankBranch" + ex.ToString());
            }
            return RedirectToAction("ListBankBranches");
        }

        public ActionResult Activate(Guid Id)
        {
            try
            {
                _bankBranchViewModelBuilder.SetActive(Id);
                TempData["msg"] = "Successfully activated";

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate bankBranch" + ex.Message);
                _log.Error("Failed to activate bankBranch" + ex.ToString());
            }
            return RedirectToAction("ListBankBranches");
        }

        public ActionResult Delete(Guid Id)
        {
            try
            {
                _bankBranchViewModelBuilder.SetDeleted(Id);
                TempData["msg"] = "Successfully Deleted";

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to Delete bankBranch" + ex.Message);
                _log.Error("Failed to Delete bankBranch" + ex.ToString());
            }
            return RedirectToAction("ListBankBranches");
        }

        public JsonResult Owner(string blogName)
        {
            IList<BankBranchViewModel> bankBranch = _bankBranchViewModelBuilder.GetAll(true);
            return Json(bankBranch);
        }
        [HttpPost]
        public ActionResult CreateBankBranch(BankBranchViewModel ctpvm)
        {
            

            try
            {
                ViewBag.BankList = _bankBranchViewModelBuilder.Bank();
                ctpvm.Id = Guid.NewGuid();
                _bankBranchViewModelBuilder.Save(ctpvm);
                TempData["msg"] = "Bank Branch Successfully created";
                return RedirectToAction("ListBankBranches");
            }
           
            catch (DomainValidationException dve)
            {

                ValidationSummary.DomainValidationErrors(dve, ModelState);
                
                _log.Debug("Failed to create bankBranch" + dve.Message);
                _log.Error("Failed to create bankBranch" + dve.ToString());
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                _log.Debug("Failed to create bankBranch" + ex.Message);
                _log.Error("Failed to create bankBranch" + ex.ToString());
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count() > 0)
                    .Select(x => new { x.Key, x.Value.Errors })
                    .ToArray();
                
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditBankBranch(BankBranchViewModel ctpvm)
        {
            try
            {
                ViewBag.BankList = _bankBranchViewModelBuilder.Bank();

                _bankBranchViewModelBuilder.Save(ctpvm);
                TempData["msg"] = "Bank Branch Successfully Edited"; 
                return RedirectToAction("ListBankBranches");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                return View();
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to edit bankBranch" + ex.Message);
                _log.Error("Failed to edit bankBranch" + ex.ToString());
                return View();
            }
        }
       
    }
}
