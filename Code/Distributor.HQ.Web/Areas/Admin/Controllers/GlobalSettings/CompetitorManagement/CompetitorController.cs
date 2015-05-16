using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CompetitorViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.CompetitorViewModel;
using Distributr.HQ.Lib.Validation;
using Distributr.Core.Domain.Master.CompetitorManagement;
using log4net;
using System.Reflection;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings
{
     [Authorize]
    public class CompetitorController : Controller
    { 
        ICompetitorViewModelBuilder _competitorViewModeBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public CompetitorController(ICompetitorViewModelBuilder competitorViewModeBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _competitorViewModeBuilder = competitorViewModeBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
          [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListCompetitors(bool showInactive = false, int page = 1, int itemsperpage = 10, string srchParam = "")
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
               /* IList<CompetitorViewModel> ls;
                ls = !string.IsNullOrWhiteSpace(searchText)
                         ? _competitorViewModeBuilder.Search(searchText, showinactive)
                         : _competitorViewModeBuilder.GetAll(showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;*/

                ViewBag.srchParam = srchParam;
                int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex * take;
                var query = new QueryStandard() { Skip = skip, Take = take, Name = srchParam, ShowInactive = showInactive };

                var ls = _competitorViewModeBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data;

                return View(data.ToPagedList(currentPageIndex,  ViewModelBase.ItemsPerPage , total));
            }
            catch (Exception ex)
            { 
                       
                _log.Debug("Failed to list competitor " + ex.Message);
                _log.Error("Failed to list competitor " + ex.ToString());
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        public ActionResult CreateCompetitor()
        {
              
            return View();
        }
       
        [HttpPost]
        public ActionResult CreateCompetitor(CompetitorViewModel cvm)
        {
            try
            {
                cvm.Id = Guid.NewGuid();
                _competitorViewModeBuilder.Save(cvm);
                TempData["msg"] = "Competitor Successfully Created";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Competitor", DateTime.Now);
                return RedirectToAction("ListCompetitors");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to create competitor " + dve.Message);
                _log.Error("Failed to create competitor " + dve.ToString());
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                _log.Debug("Failed to create competitor " + ex.Message);
                _log.Error("Failed to create competitor " + ex.ToString());
                return View();
            }
        }
          [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditCompetitor(Guid id)
        {
            try
            {
                CompetitorViewModel comp = _competitorViewModeBuilder.Get(id);
                return View(comp);
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to edit competitor " + dve.Message);
                _log.Error("Failed to edit competitor " + dve.ToString());
                return View();
            }
            catch (Exception exx)
            {
                ViewBag.msg = exx.Message;
                _log.Debug("Failed to edit competitor " + exx.Message);
                _log.Error("Failed to edit competitor " + exx.ToString());
                return View();
            }

        }
        [HttpPost]
        public ActionResult EditCompetitor(CompetitorViewModel cvm)
        {
            try
            {
                _competitorViewModeBuilder.Save(cvm);
                TempData["msg"] = "Competitor Successfully Edited";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Competitor", DateTime.Now);
                return RedirectToAction("ListCompetitors");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                return View();
            }
            catch(Exception ex)
            {
                _log.Debug("Failed to edit competitor " + ex.Message);
                _log.Error("Failed to edit competitor " + ex.ToString());
                return View();
            }
        }
        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _competitorViewModeBuilder.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Competitor", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";

            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve,ModelState);
                _log.Debug("Failed to deactivate competitor " + dve.Message);
                _log.Error("Failed to deactivate competitor " + dve.ToString());
                TempData["msg"] = dve.Message;
                return RedirectToAction("ListCompetitors");
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate competitor " + ex.Message);
                _log.Error("Failed to deactivate competitor " + ex.ToString());
                return RedirectToAction("ListCompetitors");
            }

            return RedirectToAction("ListCompetitors");
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _competitorViewModeBuilder.SetActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "Competitor", DateTime.Now);
                TempData["msg"] = "Successfully Activated";

            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to activate competitor " + dve.Message);
                _log.Error("Failed to activate competitor " + dve.ToString());
                TempData["msg"] = dve.Message;
                return RedirectToAction("ListCompetitors");
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate competitor " + ex.Message);
                _log.Error("Failed to activate competitor " + ex.ToString());
                return RedirectToAction("ListCompetitors");
            }

            return RedirectToAction("ListCompetitors");
        }
        public ActionResult Delete(Guid id)
        {
            try
            {
                _competitorViewModeBuilder.SetAsDeleted(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Competitor", DateTime.Now);
                TempData["msg"] = "Successfully Deleted";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to delete competitor " + dve.Message);
                _log.Error("Failed to delete competitor " + dve.ToString());
                TempData["msg"] = dve.Message;
                return RedirectToAction("ListCompetitors");
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to delete competitor " + ex.Message);
                _log.Error("Failed to delete competitor " + ex.ToString());
                return RedirectToAction("ListCompetitors");
            }
            return RedirectToAction("ListCompetitors");
        }
       
       
    }
}
