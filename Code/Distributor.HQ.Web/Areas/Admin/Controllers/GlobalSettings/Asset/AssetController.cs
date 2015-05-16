using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.AssetViewModelBuilders;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.ViewModels;
using log4net;
using System.Reflection;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using MvcContrib.Pagination;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModels.Admin.AssetViewModel;
using Distributr.HQ.Lib.Validation;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings.Asset
{
    public class AssetController : Controller
    {
        //
        // GET: /Admin/AssetType/
        public ActionResult Index()
        {
            return View();
        }
        IAssetViewModelBuilder _assetViewModelBuilder;
        IAssetCategoryViewModelBuilder _assetCategoryViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public AssetController(IAssetViewModelBuilder assetViewModelBuilder, IAssetCategoryViewModelBuilder assetCategoryViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _assetViewModelBuilder = assetViewModelBuilder;
            _assetCategoryViewModelBuilder = assetCategoryViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListAssets(Boolean? showInactive, string srchParam, int page = 1, int itemsperpage = 10)
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
                ViewBag.searchParam = srchParam;

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

                var ls = _assetViewModelBuilder.Query(query);

                var data = ls.Data;
                var count = ls.Count;

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list asset categories " + ex.Message);
                _log.Error("Failed to list asset categories" + ex.ToString());
                return View();
            }
        }

        public ActionResult DetailsAsset(Guid id)
        {
            try
            {
                AssetViewModel assetVM = _assetViewModelBuilder.Get(id);
                return View(assetVM);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditAsset(Guid id)
        {
            ViewBag.AssetStatusList = _assetViewModelBuilder.AssetStatus();
            ViewBag.AssetTypeList = _assetViewModelBuilder.AssetType();
            //AssetViewModel assetVM = _assetViewModelBuilder.Get(id);
            //ViewBag.AssetTypeList = _assetCategoryViewModelBuilder.GetByAssetType(assetVM.AssetTypeId);           
            ViewBag.AssetCategoryList = _assetViewModelBuilder.AssetCategory();
            try
            {

                AssetViewModel asset = _assetViewModelBuilder.Get(id);
                return View(asset);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditAsset(AssetViewModel vm)
        {
            ViewBag.AssetStatusList = _assetViewModelBuilder.AssetStatus();
            ViewBag.AssetTypeList = _assetViewModelBuilder.AssetType();            
            ViewBag.AssetCategoryList = _assetViewModelBuilder.AssetCategory();
            try
            {
                _assetViewModelBuilder.Save(vm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Asset Category", DateTime.Now);
                TempData["msg"] = "Asset Successfully Edited";
                return RedirectToAction("ListAssets");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to edit asset categories " + ve.Message);
                _log.Error("Failed to edit asset categories" + ve.ToString());

                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to edit asset categories " + ex.Message);
                _log.Error("Failed to edit asset categories" + ex.ToString());
                return View();
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateAsset()
        {
            ViewBag.AssetStatusList = _assetViewModelBuilder.AssetStatus();
            ViewBag.AssetTypeList = _assetViewModelBuilder.AssetType();
            ViewBag.AssetCategoryList = _assetViewModelBuilder.AssetCategory();

            return View("CreateAsset", new AssetViewModel());
        }

        [HttpPost]
        public ActionResult CreateAsset(AssetViewModel acvm)
        {
            ViewBag.AssetStatusList = _assetViewModelBuilder.AssetStatus();
            ViewBag.AssetTypeList = _assetViewModelBuilder.AssetType();
            ViewBag.AssetCategoryList = _assetViewModelBuilder.AssetCategory();
            try
            {
                acvm.Id = Guid.NewGuid();
                _assetViewModelBuilder.Save(acvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Asset", DateTime.Now);
                TempData["msg"] = "Asset Successfully Created";

                return RedirectToAction("ListAssets");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to create asset categories " + ve.Message);
                _log.Error("Failed to create asset categories" + ve.ToString());

                return View(acvm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to create asset categories " + ex.Message);
                _log.Error("Failed to create asset categories" + ex.ToString());

                return View(acvm);
            }

        }
        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _assetViewModelBuilder.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Asset Category", DateTime.Now);
                TempData["msg"] = "Asset Successfully Deactivated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate asset categories " + ex.Message);
                _log.Error("Failed to deactivate asset categories" + ex.ToString());
            }
            return RedirectToAction("ListAssets");
        }
        
        public JsonResult Owner(string bName)
        {
            IList<AssetViewModel> acvm = _assetViewModelBuilder.GetAll(true);
            return Json(acvm);
        }

        [HttpPost]
        public ActionResult LoadTypeAndCategories(Guid assetTypeId)
        {
            try
            {
                var categories = _assetCategoryViewModelBuilder.GetByAssetType(assetTypeId);

                if (categories.Count != 0)
                {
                    return Json(new { ok = true, data = categories, message = "ok" });
                }
                else
                {
                    return Json(new { ok = true, data = "", message = "ok" });
                }

            }
            catch (Exception exx)
            {
                _log.ErrorFormat("Error in getting routes as per Type " + exx.Message + "Asset Type Id=" + assetTypeId);
                _log.InfoFormat("Error in getting routes as per Type " + exx.Message + "Asset Type Id=" + assetTypeId);

                return Json(new { ok = false, message = exx.Message });
            }
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _assetViewModelBuilder.SetActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "Asset", DateTime.Now);
                TempData["msg"] = "Asset Successfully activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate asset" + ex.Message);
                _log.Error("Failed to activate asset" + ex.ToString());


            }

            return RedirectToAction("ListAssets");
        }

        public ActionResult Delete(Guid id)
        {
            try
            {
                _assetViewModelBuilder.SetAsDeleted(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Asset", DateTime.Now);
                TempData["msg"] = "Asset Successfully deleted";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate asset" + ex.Message);
                _log.Error("Failed to activate asset" + ex.ToString());


            }

            return RedirectToAction("ListAssets");
        }
    
    
    
    
    }
}
