using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.AssetViewModelBuilders;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Admin.AssetViewModel;
using Distributr.HQ.Lib.Validation;
using log4net;
using System.Reflection;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings.Asset
{
    public class AssetStatusController : Controller
    {
        //
        // GET: /Admin/AssetStatus/
        public ActionResult Index()
        {
            return View();
        } 
        //
        // GET: /Admin/AssetStatus/
        private IAssetStatusViewModelBuilder _assetStatusViewModelBuilder;
        private IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public AssetStatusController(IAssetStatusViewModelBuilder assetStatusViewModelBuilder,
                                     IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _assetStatusViewModelBuilder = assetStatusViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }

        public ActionResult ListAssetStatus(bool? showInactive, string srchParam, int page = 1, int itemsperpage = 10)
        {
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
                bool showinactive = false;
                if (showInactive != null)
                    showinactive = (bool) showInactive;

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
                    ShowInactive = showinactive,
                    Name = srchParam,
                    Skip = skip,
                    Take = take
                };

                var ls = _assetStatusViewModelBuilder.Query(query);

                var data = ls.Data;
                var count = ls.Count;

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list asset type" + ex.Message);
                _log.Error("Failed to list asset type" + ex.ToString());
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        public ActionResult CreateAssetStatus()
        {
            return View("CreateAssetStatus", new AssetStatusViewModel());
        }

        [HttpPost]
        public ActionResult CreateAssetStatus(AssetStatusViewModel aStatusVM)
        {
            try
            {
                aStatusVM.Id = Guid.NewGuid();
                _assetStatusViewModelBuilder.Save(aStatusVM);
                TempData["msg"] = "Asset Status Successfully Created";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "AssetStatus", DateTime.Now);
                return RedirectToAction("ListAssetStatus");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        public ActionResult EditAssetStatus(Guid id)
        {
            try
            {
                AssetStatusViewModel aStatusVm = _assetStatusViewModelBuilder.GetById(id);
                return View(aStatusVm);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditAssetStatus(AssetStatusViewModel aStatusVM)
        {
            try
            {
                _assetStatusViewModelBuilder.Save(aStatusVM);
                TempData["msg"] = "Asset Status Successfully Edited";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "AssetStatus", DateTime.Now);
                return RedirectToAction("ListAssetStatus");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _assetStatusViewModelBuilder.SetInactive(id);
                TempData["msg"] = "AssetStatus Successfully Deactivated";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "AssetStatus", DateTime.Now);
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate asset status " + ex.Message);
                _log.Error("Failed to deactivate asset status" + ex.ToString());

            }
            return RedirectToAction("ListAssetStatus");
        }

        public ActionResult Delete(Guid id)
        {
            try
            {
                _assetStatusViewModelBuilder.SetDeleted(id);
                TempData["msg"] = "AssetStatus Successfully Deleted";
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "AssetStatus", DateTime.Now);
            }
            catch(Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to delete asset status" + ex.Message);
                _log.Error("Failed to delete asset status" + ex.Message);
            }
            return RedirectToAction("ListAssetStatus");
        }

        public ActionResult Activate(Guid id, string name)
        {
            _assetStatusViewModelBuilder.SetActive(id);
            TempData["msg"] = name + " Successfully Activated";
            _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "AssetStatus", DateTime.Now);
            return RedirectToAction("ListAssetStatus");
        }

        [HttpPost]
        public ActionResult ListAssetStatus(bool? showInactive, int? page, string aStatus,string srch, int? itemsperpage)
        {
            string command = srch;
            bool showinactive = false;
            if (showInactive != null)
                showinactive = (bool)showInactive;
            
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage.Value;
                }
            ViewBag.showInactive = showinactive;

            if (TempData["msg"] != null)
            {
                ViewBag.msg = TempData["msg"].ToString();
                TempData["msg"] = null;
            }
            if (command == "Search")
            {
                var ls = _assetStatusViewModelBuilder.Search(aStatus, showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                return View(ls.ToPagedList(currentPageIndex,  ViewModelBase.ItemsPerPage));
            }
            else
              return  RedirectToAction("ListAssetStatus", new { srch = "Search", aStatus = "", showinactive = showInactive });
        }
    }   
}
