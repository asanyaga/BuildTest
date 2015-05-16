using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.DistributorTargetsViewModelBuilders;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.DistributorTargetsViewModel;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.DistributorTargetControllers
{
    [Authorize ]
    public class TargetPeriodController : Controller
    { 
        ITargetPeriodViewModelBuilder _targetPeriodViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        public TargetPeriodController(ITargetPeriodViewModelBuilder targetPeriodViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _targetPeriodViewModelBuilder = targetPeriodViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListTargetPeriods(bool? showInactive, string srchParam, int page = 1, int itemsperpage = 10)
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
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                ViewBag.SearchText = srchParam;

                int currentPageIndex = page < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex*take;

                var query = new QueryStandard()
                {
                    Name = srchParam,
                    ShowInactive = showinactive,
                    Skip = skip,
                    Take = take
                };


                var ls = _targetPeriodViewModelBuilder.Query(query);
                var data = ls.Data;
                var count = ls.Count;

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
         [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateTargetPeriod()
        {
            return View("CreateTargetPeriod", new TargetPeriodViewModel());
        }
         public ActionResult TargetPeriodDetails(Guid Id)
        {
            TargetPeriodViewModel targetPeriod = _targetPeriodViewModelBuilder.Get(Id);
            return View(targetPeriod);
        }
        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditTargetPeriod(Guid Id)
        {
            try
            {
                TargetPeriodViewModel targetPeriod = _targetPeriodViewModelBuilder.Get(Id);
                return View(targetPeriod);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditTargetPeriod(TargetPeriodViewModel tpvm)
        {
            try
            {
                _targetPeriodViewModelBuilder.Save(tpvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Target Period", DateTime.Now);
                TempData["msg"] = "Target Period Successfully Edited";
                return RedirectToAction("ListTargetPeriods");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View(tpvm);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View(tpvm);
            }
        }

        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult DeActivate(Guid Id)
        {
            try
            {
                _targetPeriodViewModelBuilder.SetInactive(Id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Target Period", DateTime.Now);
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

            }
            return RedirectToAction("ListTargetPeriods");
        }

        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult Activate(Guid Id)
        {
            try
            {
                _targetPeriodViewModelBuilder.Activate(Id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activated", "Target Period", DateTime.Now);
                TempData["msg"] = "Successfully activated";

            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListTargetPeriods");
        }

        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult Delete(Guid Id)
        {
            try
            {
                _targetPeriodViewModelBuilder.Delete(Id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deleted", "Target Period", DateTime.Now);
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

            }
            return RedirectToAction("ListTargetPeriods");
        }

        public JsonResult Owner(string blogName)
        {
            IList<TargetPeriodViewModel> targetPeriod = _targetPeriodViewModelBuilder.GetAll(true);
            return Json(targetPeriod);
        }
        [HttpPost]
        public ActionResult CreateTargetPeriod(TargetPeriodViewModel tpvm)
        {
           
            try
            {
                tpvm.Id = Guid.NewGuid();
                _targetPeriodViewModelBuilder.Save(tpvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Target Period", DateTime.Now);
                TempData["msg"] = "Target Period Successfully Created";
                return RedirectToAction("ListTargetPeriods");
            }

            catch (DomainValidationException dve)
            {

                ValidationSummary.DomainValidationErrors(dve,ModelState);
                //ViewBag.msg = msg;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;

                return View();
            }
        }
    }
}
