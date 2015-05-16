using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CostCentreViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel;
using log4net;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers
{
    public class CommodityOwnerController : Controller
    {
        private ICommodityOwnerViewModelBuilder _commodityOwnerViewModelBuilder;
        private ICommoditySupplierRepository _commoditySupplierRepository;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Guid commoditySupplier;

        public CommodityOwnerController(ICommodityOwnerViewModelBuilder commodityOwnerViewModelBuilder, ICommoditySupplierRepository commoditySupplierRepository)
        {
            _commodityOwnerViewModelBuilder = commodityOwnerViewModelBuilder;
            _commoditySupplierRepository = commoditySupplierRepository;
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListFarmers(Guid? CommoditySupplierId, Boolean? showInactive, int page=1, int itemsperpage=10, string srchParam="")
        {
            //receive temp data
            if (CommoditySupplierId != null)
            {
                ViewBag.CommoditySupplierId = CommoditySupplierId;
                var commoditysupplier = _commoditySupplierRepository.GetById(CommoditySupplierId.Value) as CommoditySupplier;
                if(commoditysupplier != null && commoditysupplier.CommoditySupplierType==CommoditySupplierType.Individual)
                {
                    ViewBag.IsFarmerEnabled = false;
                }
                else
                {
                    ViewBag.IsFarmerEnabled = true;
                }

                commoditySupplier = CommoditySupplierId.Value;
            }
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
                bool showinactive = false;
                if (showInactive != null)
                {
                    showinactive = (bool) showInactive;
                }

                ViewBag.showInactive = showinactive;
                if (TempData["msg"] != null)
                {
                    ViewBag.msg = TempData["msg"].ToString();
                    TempData["msg"] = null;
                }
                ViewBag.CommoditySupplierId = commoditySupplier;

                var currentPageIndex = page - 1 < 0 ? 0 : page - 1;
                var take = itemsperpage;
                var skip = currentPageIndex * take;

                var query = new QueryCommodityOwner();
                query.ShowInactive = showinactive;
                query.SupplierId = commoditySupplier;
                query.Name = srchParam;
                query.Take = take;
                query.Skip = skip;

                var ls = _commodityOwnerViewModelBuilder.Query(query);
                var data = ls.Data;
                var count = ls.Count;
                
                var supplier = _commoditySupplierRepository.GetById(commoditySupplier) as CommoditySupplier;
                if (supplier != null) ViewBag.CommoditySupplierType = supplier.CommoditySupplierType;

                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list hubs " + ex.Message);
                _log.Error("Failed to list hubs" + ex.ToString());
                return View();
            }
        }
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Deactivate(Guid id, Guid CommoditySupplierId)
        {
            try
            {
                _commodityOwnerViewModelBuilder.SetInactive(id);
                TempData["msg"] = "Farmer Successfully Deactivated";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate Farmer " + ex.Message);
                _log.Error("Failed to deactivate Farmer" + ex.ToString());
            }
            return RedirectToAction("ListFarmers", new { CommoditySupplierId = CommoditySupplierId });
        }
        public ActionResult Activate(Guid id, Guid CommoditySupplierId)
        {
            try
            {
                _commodityOwnerViewModelBuilder.SetActive(id);
                TempData["msg"] = "Farmer Successfully activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate farmer" + ex.Message);
                _log.Error("Failed to activate farmer" + ex.ToString());

            }

            return RedirectToAction("ListFarmers", new { CommoditySupplierId = CommoditySupplierId });
        }
       /* public ActionResult DetailsFarmer(Guid id)
        {
            try
            {
                CommodityOwnerViewModel commodityOwnerVM = _commodityOwnerViewModelBuilder.Get(id);
                return View(commodityOwnerVM);
            }
            catch (Exception ex)
            {

                ViewBag.msg = ex.Message;
                return View();
            }

        }*/

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult EditFarmer(Guid CommoditySupplierId, Guid id)
        {
            ViewBag.GenderList = _commodityOwnerViewModelBuilder.Gender();
            ViewBag.MaritalStatusList = _commodityOwnerViewModelBuilder.MaritalStatus();
            ViewBag.CommodityOwnerTypeList = _commodityOwnerViewModelBuilder.CommodityOwnerType();
            ViewBag.CommoditySupplierList = _commodityOwnerViewModelBuilder.CommoditySupplier();
            ViewBag.CommoditySupplierId = CommoditySupplierId;
            ViewBag.CommodityOwnerId = id;
            try
            {
                var query = new QueryCommodityOwner();
                query.CommodityOwnerId = id;
                query.SupplierId = CommoditySupplierId;

                CommodityOwnerViewModel commodityOwnerViewModel = _commodityOwnerViewModelBuilder.GetByQuery(query);
                return View(commodityOwnerViewModel);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }
        [HttpPost]
        public ActionResult EditFarmer(Guid CommoditySupplierId, CommodityOwnerViewModel vm)
        {
            ViewBag.GenderList = _commodityOwnerViewModelBuilder.Gender();
            ViewBag.MaritalStatusList = _commodityOwnerViewModelBuilder.MaritalStatus();
            ViewBag.CommodityOwnerTypeList = _commodityOwnerViewModelBuilder.CommodityOwnerType();
            ViewBag.CommoditySupplierList = _commodityOwnerViewModelBuilder.CommoditySupplier();
            ViewBag.CommoditySupplierId = CommoditySupplierId;
            try
            {
                _commodityOwnerViewModelBuilder.Save(vm);
                TempData["msg"] = "Farmer Successfully Edited";
                return RedirectToAction("ListFarmers", new { CommoditySupplierId = CommoditySupplierId });
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to edit Commodity owner " + ve.Message);
                _log.Error("Failed to edit Commodity owner" + ve.ToString());

                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to edit Commodity owner " + ex.Message);
                _log.Error("Failed to edit Commodity owner" + ex.ToString());
                return View();
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateFarmer(Guid CommoditySupplierId)
        {
            ViewBag.GenderList = _commodityOwnerViewModelBuilder.Gender();
            ViewBag.MaritalStatusList = _commodityOwnerViewModelBuilder.MaritalStatus();
            ViewBag.CommodityOwnerTypeList = _commodityOwnerViewModelBuilder.CommodityOwnerType();
            ViewBag.CommoditySupplierList = _commodityOwnerViewModelBuilder.CommoditySupplier();
            ViewBag.CommoditySupplierId = CommoditySupplierId;


            return View(new CommodityOwnerViewModel());
        }
        [HttpPost]
        public ActionResult CreateFarmer(Guid CommoditySupplierId, CommodityOwnerViewModel vm)
        { 
            ViewBag.GenderList = _commodityOwnerViewModelBuilder.Gender();
            ViewBag.MaritalStatusList = _commodityOwnerViewModelBuilder.MaritalStatus();
            ViewBag.CommodityOwnerTypeList = _commodityOwnerViewModelBuilder.CommodityOwnerType();
            ViewBag.CommoditySupplierList = _commodityOwnerViewModelBuilder.CommoditySupplier();
            ViewBag.CommoditySupplierId = CommoditySupplierId;

            try
            {
                vm.Id = Guid.NewGuid();
               // vm.DateOfBirth = DateTime.Now.AddYears(-10);
                vm.CommoditySupplier = CommoditySupplierId;
                _commodityOwnerViewModelBuilder.Save(vm);

                TempData["msg"] = "Farmer Successfully Created";

                return RedirectToAction("ListFarmers", new { CommoditySupplierId = CommoditySupplierId });
            }
            catch (DomainValidationException ve)
            {
                TempData["msg"] = ve.Message;
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to create commodity owners " + ve.Message);
                _log.Error("Failed to create commodity owners" + ve.ToString());

                return View(vm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to create commodity owners " + ex.Message);
                _log.Error("Failed to create commodity owners" + ex.ToString());

                return View(vm);
            }

        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult DeleteFarmer(Guid id, Guid CommoditySupplierId)
        {
            try
            {
                _commodityOwnerViewModelBuilder.SetAsDeleted(id);
                TempData["msg"] = "Farmer Successfully deleted";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to delete Farmer" + ex.Message);
                _log.Error("Failed to delete Farmer" + ex.ToString());


            }

            return RedirectToAction("ListFarmers", new { CommoditySupplierId = CommoditySupplierId });
        }
        

    }
}
