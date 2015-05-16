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
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.ViewModels.Admin.AssetViewModel;
using Distributr.HQ.Lib.Validation;

namespace Distributr.HQ.Web.Areas.Admin.Controllers.GlobalSettings.Asset
{
    [Authorize]
    public class AssetTypeController : Controller
    {
        IAssetTypeViewModelBuilder _assetTypeViewModelBuilder;
        IAuditLogViewModelBuilder _auditLogViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public AssetTypeController(IAssetTypeViewModelBuilder assetTypeViewModelBuilder, IAuditLogViewModelBuilder auditLogViewModelBuilder) 
        {
            _assetTypeViewModelBuilder = assetTypeViewModelBuilder;
            _auditLogViewModelBuilder = auditLogViewModelBuilder;
        }

        //
        // GET: /Admin/AssetType/
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListAssetTypes(bool? showInactive, string srchParam, int page = 1, int itemsperpage = 10) 
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

                var query = new QueryStandard() {ShowInactive = showinactive, Name = srchParam, Skip = skip, Take = take};

                var ls = _assetTypeViewModelBuilder.Query(query);
                var data = ls.Data;
                var count = ls.Count;

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
            }
            catch(Exception ex)
            {
                _log.Debug("Failed to list asset type" + ex.Message);
                _log.Error("Failed to list asset type" + ex.ToString());
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateAssetType()
        {
            return View("CreateAssetType", new AssetTypeViewModel());
        }

        public ActionResult AssetTypeDetails(Guid Id) 
        {
            AssetTypeViewModel assetType = _assetTypeViewModelBuilder.Get(Id);
            return View(assetType);
        }

        [Authorize(Roles = "RoleModifyMasterData")]
        public ActionResult EditAssetType(Guid Id) 
        {
            try 
            {
                AssetTypeViewModel assetType = _assetTypeViewModelBuilder.Get(Id);
                return View(assetType);
 
            }
            catch(Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
 
        }

        public ActionResult Deactivate(Guid Id) 
        {
            try
            {
                _assetTypeViewModelBuilder.SetInactive(Id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Deactivate", "Asset Type", DateTime.Now);
                TempData["msg"] = "Successfully Deactivated";
            }
            catch(DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to deactivate asset type" + dve.Message);
                _log.Error("Failed to deactivate asset type" + dve.ToString());
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate asset type" + ex.Message);
                _log.Error("Failed to deactivate asset type" + ex.ToString());
            }
            return RedirectToAction("ListAssetTypes");
        }

        public ActionResult Delete(Guid Id)
        {
            try
            {
                _assetTypeViewModelBuilder.SetDeleted(Id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Delete", "Asset Type", DateTime.Now);
                TempData["msg"] = "Successfully Deleted";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to Delete asset type" + dve.Message);
                _log.Error("Failed to Delete asset type" + dve.ToString());
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to Delete asset type" + ex.Message);
                _log.Error("Failed to Delete asset type" + ex.ToString());
            }
            return RedirectToAction("ListAssetTypes");
        }

        [HttpPost]
        public ActionResult CreateAssetType(AssetTypeViewModel atvm) 
        {
            try
            {
                atvm.Id = Guid.NewGuid();
                _assetTypeViewModelBuilder.Save(atvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Create", "AssetType", DateTime.Now);
                TempData["msg"] = "Asset Type Successfully Created";
                return RedirectToAction("ListAssetTypes");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                //ViewBag.msg = dve.Message;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }

        }

        [HttpPost]
        public ActionResult EditAssetType(AssetTypeViewModel atvm) 
        {
            try
            {
                _assetTypeViewModelBuilder.Save(atvm);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Edit", "Asset Type", DateTime.Now);
                TempData["msg"] = "Asset Type Successfully Edited";
                return RedirectToAction("ListAssetTypes");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View();
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to edit asset type" + ex.Message);
                _log.Error("Failed to edit asset type" + ex.ToString());
                return View();
            }
        }

        public ActionResult Activate(Guid Id)
        {
            try
            {
                _assetTypeViewModelBuilder.SetActive(Id);
                _auditLogViewModelBuilder.AddAuditLog(this.User.Identity.Name, "Activate", "Asset Type", DateTime.Now);
                TempData["msg"] = "Successfully Activated";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to activate asset type" + dve.Message);
                _log.Error("Failed to activate asset type" + dve.ToString());
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate asset type" + ex.Message);
                _log.Error("Failed to activate asset type" + ex.ToString());
            }
            return RedirectToAction("ListAssetTypes");
        }


    }
}


