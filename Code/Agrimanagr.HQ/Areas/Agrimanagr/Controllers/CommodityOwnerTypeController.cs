using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.Transaction;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CostCentreViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel;
using log4net;
using Distributr.HQ.Lib.Paging;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers
{
  
    public class CommodityOwnerTypeController : Controller
    {
        ICommodityOwnerTypeViewModelBuilder _ownerTypeViewModelBuilder;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CommodityOwnerTypeController(ICommodityOwnerTypeViewModelBuilder ownerTypeViewModelBuilder)
        {
            _ownerTypeViewModelBuilder = ownerTypeViewModelBuilder;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListCommodityOwnerTypes(bool? showInactive, string srchParam="", int page = 1, int itemsperpage = 10)
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
                ViewBag.srchParam = srchParam;

                int currentPageIndex = page < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = currentPageIndex*take;

                var query = new QueryStandard(){Name = srchParam, ShowInactive = showinactive, Skip = skip, Take = take};

                var result = _ownerTypeViewModelBuilder.Query(query);
                var count = result.Count;
                var data = result.Data;
                return View(data.ToPagedList(currentPageIndex,  ViewModelBase.ItemsPerPage, count));
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list CommodityOwner type" + ex.Message);
                _log.Error("Failed to list CommodityOwner type" + ex.ToString());
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        public ActionResult Details(Guid id)
        {
            CommodityOwnerTypeViewModel typeViewModel = _ownerTypeViewModelBuilder.Get(id);
            return View(typeViewModel);
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult Create()
        {
            return View("Create", new CommodityOwnerTypeViewModel());
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult Edit(Guid id)
        {
            try
            {
                CommodityOwnerTypeViewModel commodityOwnerTypeViewModel = _ownerTypeViewModelBuilder.Get(id);
                return View(commodityOwnerTypeViewModel);

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
                _ownerTypeViewModelBuilder.SetInactive(id);
                TempData["msg"] = "Successfully Deactivated";


            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;

            }
            return RedirectToAction("ListCommodityOwnerTypes");
        }

        public JsonResult Owner(string bName)
        {
            IList<CommodityOwnerTypeViewModel> tvm = _ownerTypeViewModelBuilder.GetAll(true);
            return Json(tvm);
        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult Delete(Guid Id)
        {
            try
            {
                _ownerTypeViewModelBuilder.SetAsDeleted(Id);
                TempData["msg"] = "Successfully Deleted";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to Delete CommodityOwnerType" + dve.Message);
                _log.Error("Failed to Delete CommodityOwnerType" + dve.ToString());
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to Delete CommodityOwnerType" + ex.Message);
                _log.Error("Failed to Delete CommodityOwnerType" + ex.ToString());
            }
            return RedirectToAction("ListCommodityOwnerTypes");
        }

        public ActionResult Activate(Guid Id)
        {
            try
            {
                _ownerTypeViewModelBuilder.SetActive(Id);
                TempData["msg"] = "Successfully Activated";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                _log.Debug("Failed to activate CommodityOwnerType" + dve.Message);
                _log.Error("Failed to activate CommodityOwnerType" + dve.ToString());
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate CommodityOwnerType" + ex.Message);
                _log.Error("Failed to activate CommodityOwnerType" + ex.ToString());
            }
            return RedirectToAction("ListCommodityOwnerTypes");
        }

        [HttpPost]
        public ActionResult Create(CommodityOwnerTypeViewModel atvm)
        {
            try
            {
                atvm.Id = Guid.NewGuid();
                _ownerTypeViewModelBuilder.Save(atvm);
                TempData["msg"] = "CommodityOwnerType Successfully Created";
                return RedirectToAction("ListCommodityOwnerTypes");
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

        [HttpPost]
        public ActionResult Edit(CommodityOwnerTypeViewModel atvm)
        {
            try
            {
                _ownerTypeViewModelBuilder.Save(atvm);
           
                TempData["msg"] = "CommodityOwnerType Successfully Edited";
                return RedirectToAction("ListCommodityOwnerTypes");
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                return View();
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to edit CommodityOwnerType" + ex.Message);
                _log.Error("Failed to edit CommodityOwnerType" + ex.ToString());
                return View();
            }
        }

    }
}
