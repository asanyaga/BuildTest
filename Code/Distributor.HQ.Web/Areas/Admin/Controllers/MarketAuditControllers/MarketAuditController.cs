using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.MarketAuditViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.MarketAuditViewModels ;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.MarketAuditControllers
{
    [Authorize ]
    public class MarketAuditController : Controller
    {
        IMarketAuditViewModelBuilder _marketAuditViewmodelbuilder;
        public MarketAuditController(IMarketAuditViewModelBuilder marketAuditViewModelBuilder)
        {
            _marketAuditViewmodelbuilder = marketAuditViewModelBuilder;
        }
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListMarketAudits(bool? showInactive, string srchParam, int page = 1, int itemsperpage = 10)
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

            int currentPageIndex = page < 0 ? 0 : page - 1;
            int take = itemsperpage;
            int skip = currentPageIndex*take;

            var query = new QueryStandard() {ShowInactive = showinactive, Name = srchParam, Skip = skip, Take = take};


            var ls = _marketAuditViewmodelbuilder.Query(query);
            var data = ls.Data;
            var count = ls.Count;

            return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
           
        }
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateMarketAudit()
        {
            return View("CreateMarketAudit", new MarketAuditViewModel());
        }

        public ActionResult MarketAuditDetails(Guid Id)
        {
            MarketAuditViewModel marketAudit = _marketAuditViewmodelbuilder.Get(Id);
            return View(marketAudit);
        }
        [Authorize(Roles = "RoleMofidyMasterData")]
        public ActionResult EditMarketAudit(Guid Id)
        {
            MarketAuditViewModel marketAudit = _marketAuditViewmodelbuilder.Get(Id);
            return View(marketAudit);
        }
        public ActionResult DeActivate(Guid Id)
        {
            try
            {
                _marketAuditViewmodelbuilder.SetInactive(Id);
                TempData["msg"] = "Successfully Deactivated";

            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListMarketAudits");
        }

        public JsonResult Owner(string blogName)
        {
            IList<MarketAuditViewModel> marketAudit = _marketAuditViewmodelbuilder.GetAll(true);
            return Json(marketAudit);
        }
        [HttpPost]
        public ActionResult CreateMarketAudit(MarketAuditViewModel tpvm)
        {
           
            try
            {
                tpvm.Id = Guid.NewGuid();
                _marketAuditViewmodelbuilder.Save(tpvm);
                return RedirectToAction("ListMarketAudits");
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
        [HttpPost]
        public ActionResult EditMarketAudit(MarketAuditViewModel tpvm)
        {
            _marketAuditViewmodelbuilder.Save(tpvm);
            return RedirectToAction("ListMarketAudits");
        }
        [HttpPost]
        public ActionResult ListMarketAudits(bool? showInactive, int? page, string srch, string distName, int? itemsperpage)
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
            var ls = _marketAuditViewmodelbuilder.Search(distName, showinactive);
            int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
            if (command == "Search")
            {
                return View(ls.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
            }
            else
            {
                return RedirectToAction("ListMarketAudits", new { showinactive = showInactive, srch = "Search", distName = "" });
            }
        }
    }

    }