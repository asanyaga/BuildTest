using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.MaritalStatusViewModelBuilders;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModels.Admin.MaritalStatusViewModels;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings.MaritalStatus
{
    public class MaritalStatusController : Controller
    {
        private const int defaultPageSize = 10;
        //
        // GET: /Admin/MaritalStatus/
        IMaritalStatusViewModelBuilder _maritalStatusViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        public MaritalStatusController(IMaritalStatusViewModelBuilder maritalStatusViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _maritalStatusViewModelBuilder = maritalStatusViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult ListMaritalStatus(bool? showInactive,int? page)
        {
            try
            {
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool)showInactive;

                ViewBag.showInactive = showinactive;

                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                var ls = _maritalStatusViewModelBuilder.GetAll(showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                return View(ls.ToPagedList(currentPageIndex, defaultPageSize));
            }
            catch (Exception ex)
            {
                var exception = new MaritalStatusViewModel();
                exception.ErrorText = ex.Message;
                return View(exception);
            }
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CreateMaritalStatus()
        {
            return View("CreateMaritalStatus",new MaritalStatusViewModel());
        }
        [HttpPost]
        public ActionResult CreateMaritalStatus(MaritalStatusViewModel mStatusVM)
        {
            try
            {
                mStatusVM.Id = Guid.NewGuid();
                _maritalStatusViewModelBuilder.Save(mStatusVM);
                TempData["msg"] = "MaritalStatus Successfully Created";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "MaritalStatus", DateTime.Now);
               return RedirectToAction("ListMaritalStatus");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = "";
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        public ActionResult EditMaritalStatus(Guid id)
        {
            try
            {
                MaritalStatusViewModel mStatusVm = _maritalStatusViewModelBuilder.GetById(id);
                return View(mStatusVm);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditMaritalStatus(MaritalStatusViewModel mStatusVM)
        {
            try
            {
                _maritalStatusViewModelBuilder.Save(mStatusVM);
                TempData["msg"] = "MaritalStatus Successfully Edited";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "MaritalStatus", DateTime.Now);
               return RedirectToAction("ListMaritalStatus");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = "";
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        public ActionResult Deactivate(Guid id)
        {
            _maritalStatusViewModelBuilder.SetInactive(id);
            TempData["msg"] = "MaritalStatus Successfully Deactivated";
            _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "MaritalStatus", DateTime.Now);
            return RedirectToAction("ListMaritalStatus");
        }

        public ActionResult Delete(Guid id)
        {
            _maritalStatusViewModelBuilder.SetDeleted(id);
            TempData["msg"] = "MaritalStatus Successfully Deleted";
            _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "MaritalStatus", DateTime.Now);
            return RedirectToAction("ListMaritalStatus");
        }

        public ActionResult Activate(Guid id, string name)
        {
            _maritalStatusViewModelBuilder.SetActive(id);
            TempData["msg"] = name + " Successfully Activated";
            _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "MaritalStatus", DateTime.Now);
            return RedirectToAction("ListMaritalStatus");
        }

        [HttpPost]
        public ActionResult ListMaritalStatus(bool? showInactive, int? page, string mStatus,string srch)
        {
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
            if (command == "Search")
            {
                var ls = _maritalStatusViewModelBuilder.Search(mStatus, showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                return View(ls.ToPagedList(currentPageIndex, defaultPageSize));
            }
            else
              return  RedirectToAction("ListMaritalStatus", new { srch = "Search", mStatus = "", showinactive = showInactive });
        }
    }
}
