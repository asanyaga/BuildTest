using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.CostCenter;
using Distributr.HQ.Lib.Validation;
using MvcContrib.Pagination;
using log4net;
using System.Reflection;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.Paging;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.CostCenter
{
    [Authorize]
    public class TerritoryController : Controller
    { 
        ITerritoryViewModelBuilder _territoryViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public TerritoryController(ITerritoryViewModelBuilder territoryViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _territoryViewModelBuilder = territoryViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListTerritory(Boolean? showInactive, int page = 1, int itemsperpage = 10, string srchParam = "")
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
/*
                var ls = _territoryViewModelBuilder.GetAll(showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
                //return View(ls);*/
                ViewBag.srchParam = srchParam;
                 var currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                var take = itemsperpage;
                var skip = currentPageIndex * take;
                var query = new QueryStandard { Name = srchParam, ShowInactive = showinactive, Skip = skip, Take = take };
                var ls = _territoryViewModelBuilder.Query(query);
                var total = ls.Count;
                var data = ls.Data;

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, total));
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list territory" + ex.Message);
                _log.Error("Failed to list territory" + ex.ToString());
                return View();
            }
        }

        public ActionResult DetailsTerritory(Guid id)
        {
            TerritoryViewModel territory = _territoryViewModelBuilder.Get(id);
            return View(territory);
        }

       
        //  /Admin/Territory/Create
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateTerritory()
        {
            return View("CreateTerritory",new TerritoryViewModel());
        } 

    
        // POST: /Admin/Territory/Create

        [HttpPost]
        public ActionResult CreateTerritory(TerritoryViewModel territory)
        {
            try
            {
                territory.Id = Guid.NewGuid();
                _territoryViewModelBuilder.Save(territory);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Country Region", DateTime.Now);
                TempData["msg"] = "Country Region Created Successfully";
                return RedirectToAction("ListTerritory");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to create territory" + dve.Message);
                _log.Error("Failed to create territory" + dve.ToString());
                return View();
            }
            catch(Exception exx)
            {
                ViewBag.msg = exx.Message;
                _log.Debug("Failed to create territory" + exx.Message);
                _log.Error("Failed to create territory" + exx.ToString());
                return View();
            }
        }
        
        //
        // GET: /Admin/Territory/Edit/5
        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditTerritory(Guid id)
        {
            TerritoryViewModel territory = _territoryViewModelBuilder.Get(id);
            return View(territory);
        }

        //
        // POST: /Admin/Territory/Edit/5

        [HttpPost]
        public ActionResult EditTerritory(TerritoryViewModel territory)
        {
            try
            {
                _territoryViewModelBuilder.Save(territory);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Country Region", DateTime.Now);
                TempData["msg"] = "Country Region Edited Successfully";
                return RedirectToAction("ListTerritory");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to edit territory" + dve.Message);
                _log.Error("Failed to edit territory" + dve.ToString());
                return View();
            }
            catch(Exception exx)
            {
                ViewBag.msg = exx.Message;
                _log.Debug("Failed to edit territory" + exx.Message);
                _log.Error("Failed to edit territory" + exx.ToString());
                return View();
            }
        }
     
        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _territoryViewModelBuilder.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Country Region", DateTime.Now);
                TempData["msg"] = "Country Region Deactivate Successfully";
                }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate territory" + ex.Message);
                _log.Error("Failed to deactivate territory" + ex.ToString());
            }
            return RedirectToAction("ListTerritory");
        }

        public ActionResult Activate(Guid id, string name)
        {
            try
            {
                _territoryViewModelBuilder.SetActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "Country Region", DateTime.Now);
                TempData["msg"] = name + " Country-Region Activated Successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate territory" + ex.Message);
                _log.Error("Failed to activate territory" + ex.ToString());
            }
            return RedirectToAction("ListTerritory");
        }
        public ActionResult Delete(Guid id, string name)
        {
            try
            {
                _territoryViewModelBuilder.SetAsDeleted(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Country Region", DateTime.Now);
                TempData["msg"] = name + " country region deleted successfully";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to delete territory" + ex.Message);
                _log.Error("Failed to delete territory" + ex.ToString());
            }
            return RedirectToAction("ListTerritory");
        }
        public JsonResult Owner(string bName)
        {
            IList<TerritoryViewModel> tvm = _territoryViewModelBuilder.GetAll(true);
            return Json(tvm);
        }
       
    }
}
