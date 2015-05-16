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
using Distributr.HQ.Lib.Paging;
using log4net;
using System.Reflection;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings.Regions
{
     [Authorize]
    public class CountryRegionController : Controller
    {
        //
        // GET: /Admin/CountryRegion/

        ITerritoryViewModelBuilder _territoryViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public CountryRegionController(ITerritoryViewModelBuilder territoryViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _territoryViewModelBuilder = territoryViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListTerritory(Boolean? showInactive, int itemsperpage = 10, int page = 1, string srchParam = "")
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
                ViewBag.searchText = srchParam;

                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }

                int currentPageIndex = page;
                int take = itemsperpage;
                int skip = currentPageIndex*take;

                var query = new QueryStandard()
                {
                    Name = srchParam,
                    ShowInactive = showinactive,
                    Skip = skip,
                    Take = take
                };

                var ls = _territoryViewModelBuilder.Query(query);
                var data = ls.Data;
                var count = ls.Count;
                
                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list country region" + ex.Message);
                _log.Error("Failed to list country region" + ex.ToString());
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
                TempData["msg"] = "Country Region Successfully Created";
                return RedirectToAction("ListTerritory");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to create country region" + dve.Message);
                _log.Error("Failed to create country region" + dve.ToString());
                return View();
            }
            catch(Exception exx)
            {
                ViewBag.msg = exx.Message;
                _log.Debug("Failed to list country region" + exx.Message);
                _log.Error("Failed to list country region" + exx.ToString());
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
                TempData["msg"] = "Country Region Successfully Edited";
                return RedirectToAction("ListTerritory");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to edit country region" + dve.Message);
                _log.Error("Failed to edit country region" + dve.ToString());
                return View();
            }
            catch(Exception exx)
            {
                ViewBag.msg = exx.Message;
                _log.Debug("Failed to edit country region" + exx.Message);
                _log.Error("Failed to edit country region" + exx.ToString());
                return View();
            }
        }
        public JsonResult Owner(string bName)
        {
            IList<TerritoryViewModel> tvm = _territoryViewModelBuilder.GetAll(true);
            return Json(tvm);
        }
       
    }
}
