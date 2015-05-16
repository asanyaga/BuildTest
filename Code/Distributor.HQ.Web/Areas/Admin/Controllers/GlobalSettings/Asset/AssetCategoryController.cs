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
using Distributr.HQ.Lib.ViewModels.Admin.AssetViewModel;
using Distributr.HQ.Lib.Validation;
using MvcContrib.Pagination;
using Distributr.HQ.Lib.Paging;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings.Asset
{
    public class AssetCategoryController : Controller
    {
        //
        // GET: /Admin/AssetCategory/
        public ActionResult Index()
        {
            return View();
        }
        IAssetCategoryViewModelBuilder _assetCategoryViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;        
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public AssetCategoryController(IAssetCategoryViewModelBuilder assetCategoryViewModelBuilder,IAuditLogViewModelBuilder auditLogViewModelBuilder)
        {
            _assetCategoryViewModelBuilder = assetCategoryViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }
       


        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListAssetCategories(Boolean? showInactive, string srchParam,int page = 1, int itemsperpage = 10)
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

                int currentPageIndex = page < 0 ? 0 :page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex*take;

                var query = new QueryStandard()
                {
                    Name = srchParam,
                    ShowInactive = showinactive,
                    Take = take,
                    Skip = skip
                };

                var ls = _assetCategoryViewModelBuilder.Query(query);

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

        public ActionResult DetailsAssetCategory(Guid id)
        {
            try
            {
                AssetCategoryViewModel ocVM = _assetCategoryViewModelBuilder.Get(id);
                return View(ocVM);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
         [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditAssetCategory(Guid id)
        {
            ViewBag.AssetTypeList = _assetCategoryViewModelBuilder.AssetType();
            try
            {
                
                AssetCategoryViewModel assetCategory = _assetCategoryViewModelBuilder.Get(id);
                return View(assetCategory);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditAssetCategory(AssetCategoryViewModel vm)
        {
            ViewBag.AssetTypeList = _assetCategoryViewModelBuilder.AssetType(); 
            try
            {
                _assetCategoryViewModelBuilder.Save(vm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Asset Category", DateTime.Now);
                TempData["msg"] = "Asset Category Successfully Edited";

                return RedirectToAction("ListAssetCategories");
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
        public ActionResult CreateAssetCategory()
        {
            ViewBag.AssetTypeList = _assetCategoryViewModelBuilder.AssetType();
            return View("CreateAssetCategory", new AssetCategoryViewModel());
        }

        [HttpPost]
        public ActionResult CreateAssetCategory(AssetCategoryViewModel acvm)
        {
            ViewBag.AssetTypeList = _assetCategoryViewModelBuilder.AssetType();
            try
            {
                acvm.Id = Guid.NewGuid();
                _assetCategoryViewModelBuilder.Save(acvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "Province", DateTime.Now);
                //_auditLogViewModelBuilder.addAuditLog(this.User.Identity.Name, "Create", "Asset Category", DateTime.Now);
                TempData["msg"] = "Asset Category Successfully Created";
                return RedirectToAction("listassetcategories");
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
                _assetCategoryViewModelBuilder.SetInactive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Asset Category", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate asset categories " + ex.Message);
                _log.Error("Failed to deactivate asset categories" + ex.ToString());
               
            }
            return RedirectToAction("ListAssetCategories");
        }

        public ActionResult Delete(Guid id)
        {

            try
            {
                _assetCategoryViewModelBuilder.SetDeleted(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Asset Category", DateTime.Now);
                TempData["msg"] = "Successfully Deleted";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to Delete asset categories " + ex.Message);
                _log.Error("Failed to Delete asset categories" + ex.ToString());

            }
            return RedirectToAction("ListAssetCategories");
        }

        public ActionResult Activate(Guid id, string name)
        {
            try
            {
                _assetCategoryViewModelBuilder.SetActive(id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "AssetCategory", DateTime.Now);
                TempData["msg"] = name + " Successfully Activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate asset category" + ex.Message);
                _log.Error("Failed to deactivate asset category" + ex.ToString());
            }
            return RedirectToAction("ListAssetCategories");
        }
        [HttpPost]
        public ActionResult ListAssetCategories(Boolean? showInactive, int? page, string srch, string aCategory, int? itemsperpage)
        {
            try
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

                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }

                if (command == "Search")
                {
                    var assetCateg = _assetCategoryViewModelBuilder.Search(aCategory, showinactive);
                    int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                    return View(assetCateg.ToPagedList(currentPageIndex,  ViewModelBase.ItemsPerPage));
                    //return View(assetCategPagedListContainer);
                }
                else
                {
                    return RedirectToAction("ListAssetCategories", new { srch = "Search", aCategory = "", showinactive = showInactive });
                }
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        public JsonResult Owner(string bName)
        {
            IList<AssetCategoryViewModel> acvm = _assetCategoryViewModelBuilder.GetAll(true);
            return Json(acvm);
        } 
       
    }
}
