using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModelBuilders.DistributorTargetsViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.DistributorTargetsViewModelBuilders.Impl;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.DistributorTargetsViewModel;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using log4net;
using System.Reflection;
using System.Configuration;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.DistributorTargetControllers
{
    [Authorize ]
    public class TargetController : Controller
    {
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        ITargetViewModelBuilder _targetViewModelBuilder; 
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        
        //
        // GET: /Admin/Target/
        public TargetController(TargetViewModelBuilder targetViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _targetViewModelBuilder = targetViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
            //_hqMailerViewModelBuilder = hqMailerViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
         [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListTargets(bool? showInactive, string searchText, int page = 1, int itemsperpage = 10)
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

                int currentPageIndex = page < 0 ? 0 : page -1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;

                var query = new QueryStandard(){Name = searchText, ShowInactive = showinactive, Skip = skip, Take = take};

                var ls = _targetViewModelBuilder.Query(query);
                var data = ls.Data;
                var count = ls.Count;

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));

            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
            _log.Error("Loading distributor targets"+ex.ToString());
            try
            {
                HQMailerViewModelBuilder hqm = new HQMailerViewModelBuilder(ConfigurationSettings.AppSettings["ServerIP"], ConfigurationSettings.AppSettings["UserName"], ConfigurationSettings.AppSettings["Password"]);


                hqm.Send(ConfigurationSettings.AppSettings["ServerEmail"], ConfigurationSettings.AppSettings["MailGroup"], "Test", ex.Message);
            }
                catch(Exception exx){}
                return View();
            }
        }
         [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateTarget()
        {
            ViewBag.DistributorList = _targetViewModelBuilder.GetDistributor();
            ViewBag.PeriodList = _targetViewModelBuilder.GetPeriod();
            return View("CreateTarget", new TargetViewModel());
        }
        [HttpPost]
        public ActionResult CreateTarget(TargetViewModel tvm)
        {
            ViewBag.DistributorList = _targetViewModelBuilder.GetDistributor();
            ViewBag.PeriodList = _targetViewModelBuilder.GetPeriod();
            try
            {
                tvm.Id = Guid.NewGuid();
                _targetViewModelBuilder.Save(tvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Target", DateTime.Now);
                TempData["msg"] = "Target Successfully Created";
                return RedirectToAction("ListTargets");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View();
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
                ValidationSummary.DomainValidationErrors(exx.Message, ModelState);
                return View();
            }
        }
        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditTarget(Guid id)
        {
            ViewBag.DistributorList = _targetViewModelBuilder.GetDistributor();
            ViewBag.PeriodList = _targetViewModelBuilder.GetPeriod();
            TargetViewModel tvm = _targetViewModelBuilder.GetById(id);
            return View(tvm);
        }
        [HttpPost]
        public ActionResult EditTarget(TargetViewModel tvm)
        {
            ViewBag.DistributorList = _targetViewModelBuilder.GetDistributor();
            ViewBag.PeriodList = _targetViewModelBuilder.GetPeriod();
            try
            {
                _targetViewModelBuilder.Save(tvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Target", DateTime.Now);
                TempData["msg"] = "Target Successfully Edited";
                return RedirectToAction("ListTargets");
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
        public JsonResult Owner(string blogName)
        {
            IList<TargetViewModel> target= _targetViewModelBuilder.GetAll(true);
            return Json(target);
        }

        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult DeActivate(Guid Id)
        {
            try
            {
                _targetViewModelBuilder.SetInactive(Id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Target", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListTargets");
        }

        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult Activate(Guid Id)
        {
            try
            {
                _targetViewModelBuilder.Activate(Id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "Target", DateTime.Now);
                TempData["msg"] = "Successfully activated";

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListTargets");
        }

        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult Delete(Guid Id)
        {
            try
            {
                _targetViewModelBuilder.Delete(Id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Target", DateTime.Now);
                TempData["msg"] = "Successfully deleted";

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListTargets");
        }
    }
}
