using System;
using System.Reflection;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CommodityViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CommodityViewModel;
using log4net;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers
{
    public class CommodityTypeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        ICommodityTypeViewModelBuilder _commodityTypeViewModelBuilder;
         protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
         public CommodityTypeController(ICommodityTypeViewModelBuilder commodityTypeViewModelBuilder)
         {
             _commodityTypeViewModelBuilder = commodityTypeViewModelBuilder;
         }

         [Authorize(Roles = "RoleViewMasterData")]
         public ActionResult ListCommodityTypes(Boolean? showInactive, int page=1, int itemsperpage=10,string srchparam="")
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
                 ViewBag.srchparam = srchparam;
                 ViewBag.showInactive = showinactive;
                 if (TempData["msg"] != null)
                 {
                     ViewBag.msg = TempData["msg"].ToString();
                     TempData["msg"] = null;
                 }
                 //var ls = _commodityTypeViewModelBuilder.GetAll(showinactive);
                 //int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                 int currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                 int take = itemsperpage;
                 int skip = currentPageIndex*take;
                 var query = new QueryStandard();
                 query.ShowInactive = showinactive;
                 query.Name = srchparam;
                 query.Skip = skip;
                 query.Take = take;

                 var result = _commodityTypeViewModelBuilder.Query(query);
                 var total=result.Count;
                 var data = result.Data;

                 return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, total));
             }
             catch (Exception ex)
             {
                 _log.Debug("Failed to list commodity types " + ex.Message);
                 _log.Error("Failed to list commodity types" + ex.ToString());
                 return View();
             }
         }

        [Authorize(Roles = "RoleViewMasterData")]
         public ActionResult DetailsCommodityType(Guid id)
         {
             try
             {
                 CommodityTypeViewModel commidityTypeVM = _commodityTypeViewModelBuilder.Get(id);
                 return View(commidityTypeVM);
             }
             catch (Exception ex)
             {
                 ViewBag.msg = ex.Message;
                 return View();
             }
         }

        [Authorize(Roles = "RoleUpdateMasterData")]
         public ActionResult EditCommodityType(Guid id)
         {
             ViewBag.RegionList = _commodityTypeViewModelBuilder.Region();
             
             try
             {

                 CommodityTypeViewModel commodityTypeViewModel = _commodityTypeViewModelBuilder.Get(id);
                 return View(commodityTypeViewModel);
             }
             catch (Exception ex)
             {
                 ViewBag.msg = ex.Message;
                 return View();
             }
         }

         [HttpPost]
         public ActionResult EditCommodityType(CommodityTypeViewModel vm)
         {
             ViewBag.RegionList = _commodityTypeViewModelBuilder.Region();
             try
             {
                 _commodityTypeViewModelBuilder.Save(vm);
                 TempData["msg"] = "Asset Successfully Edited";
                 return RedirectToAction("ListCommodityTypes");
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
         public ActionResult CreateCommodityType()
         {
             ViewBag.RegionList = _commodityTypeViewModelBuilder.Region();

             return View( new CommodityTypeViewModel());
         }

         [HttpPost]
         public ActionResult CreateCommodityType(CommodityTypeViewModel vm)
         {
             ViewBag.RegionList = _commodityTypeViewModelBuilder.Region();
             
             try
             {
                 vm.Id = Guid.NewGuid();
                 _commodityTypeViewModelBuilder.Save(vm);
                 
                 TempData["msg"] = "Commodity Type Successfully Created";

                 return RedirectToAction("ListCommodityTypes");
             }
             catch (DomainValidationException ve)
             {
                 ValidationSummary.DomainValidationErrors(ve, ModelState);
                 _log.Debug("Failed to create asset categories " + ve.Message);
                 _log.Error("Failed to create asset categories" + ve.ToString());

                 return View(vm);
             }
             catch (Exception ex)
             {
                 ModelState.AddModelError("", ex.Message);
                 _log.Debug("Failed to create asset categories " + ex.Message);
                 _log.Error("Failed to create asset categories" + ex.ToString());

                 return View(vm);
             }

         }
         /*public ActionResult Deactivate(Guid id)
         {
             try
             {
                 _commodityTypeViewModelBuilder.SetInactive(id);
                 
                 TempData["msg"] = "Asset Successfully Deactivated";
             }
             catch (Exception ex)
             {
                 TempData["msg"] = ex.Message;
                 _log.Debug("Failed to deactivate commodity type " + ex.Message);
                 _log.Error("Failed to deactivate commodity type" + ex.ToString());
             }
             return RedirectToAction("ListAssets");
         }
         [HttpPost]
         public ActionResult ListCommodityTypes(Boolean? showInactive, int? page, string srch, string srchParam)
         {
             try
             {
                 int pageSize = 10;
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
                     var ls = _commodityTypeViewModelBuilder.SearchCommodityTypes(srchParam, showinactive);
                     int currentPageIndex = page.HasValue ? page.Value - 1 : 0;
                     return View(ls.ToPagedList(currentPageIndex, pageSize));
                 }
                 else
                     return RedirectToAction("ListCommodityTypes", new { srchParam = "", showinactive = showInactive, srch = "Search" });
             }
             catch (Exception ex)
             {
                 ViewBag.msg = ex.Message;
                 return View();
             }
         }
         public JsonResult Owner(string bName)
         {
             IList<CommodityTypeViewModel> vm = _commodityTypeViewModelBuilder.GetAll(true);
             return Json(vm);
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
                 _commodityTypeViewModelBuilder.SetActive(id);
                TempData["msg"] = "Asset Successfully activated";
             }
             catch (Exception ex)
             {
                 TempData["msg"] = ex.Message;
                 _log.Debug("Failed to activate commodity type" + ex.Message);
                 _log.Error("Failed to activate commodity type" + ex.ToString());


             }

             return RedirectToAction("ListCommodityTypes");
         }*/

        [Authorize(Roles = "RoleDeleteMasterData")]
         public ActionResult DeleteCommodityType(Guid id)
         {
             try
             {
                 _commodityTypeViewModelBuilder.SetAsDeleted(id);
                 TempData["msg"] = "Commodity type Successfully deleted";
             }
             catch (DomainValidationException dve)
             {
                 ValidationSummary.DomainValidationErrors(dve, ModelState);
                 TempData["msg"] = dve.Message;
             }
             catch (Exception ex)
             {
                 TempData["msg"] = ex.Message;
                 _log.Debug("Failed to activate commodity type" + ex.Message);
                 _log.Error("Failed to activate commodity type" + ex.ToString());
             }

             return RedirectToAction("ListCommodityTypes");
         }
         public ActionResult Deactivate(Guid id)
         {
             try
             {
                 _commodityTypeViewModelBuilder.SetInactive(id);
                 TempData["msg"] = "Commodity type successfully Deactivated";
             }
             catch (DomainValidationException dve)
             {
                 ValidationSummary.DomainValidationErrors(dve, ModelState);
                 TempData["msg"] = dve.Message;
             }
             catch (Exception ex)
             {
                 TempData["msg"] = ex.Message;
                 _log.Debug("Failed to deactivate commodity type " + ex.Message);
                 _log.Error("Failed to deactivate commodity type" + ex.ToString());
             }
             return RedirectToAction("ListCommodityTypes");
         }
         public ActionResult Activate(Guid id)
         {
             try
             {
                 _commodityTypeViewModelBuilder.SetActive(id);
                 TempData["msg"] = "Commodity type successfully activated";
             }
             catch (Exception ex)
             {
                 TempData["msg"] = ex.Message;
                 _log.Debug("Failed to activate Commodity type" + ex.Message);
                 _log.Error("Failed to activate Commodity type" + ex.ToString());

             }

             return RedirectToAction("ListCommodityTypes");
         }
    
    
    
    
     }
    }

