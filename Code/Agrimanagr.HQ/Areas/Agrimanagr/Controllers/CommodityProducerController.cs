using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.MasterDataAllocationRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Admin.CostCenter;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CostCentreViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel;
using log4net;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers
{
    public class CommodityProducerController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        private readonly ICommodityProducerViewModelBuilder _commodityProducerViewModelBuilder;
        private readonly ICentreRepository _centreRepository;
        private readonly IMasterDataAllocationRepository _masterDataAllocationRepository;
        private readonly ICommoditySupplierRepository _commoditySupplierRepository;
        private static Guid commoditySupplier;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public CommodityProducerController(ICommodityProducerViewModelBuilder commodityProducerViewModelBuilder, ICentreRepository centreRepository, IMasterDataAllocationRepository masterDataAllocationRepository, ICommoditySupplierRepository commoditySupplierRepository)
        {
            _commodityProducerViewModelBuilder = commodityProducerViewModelBuilder;
            _centreRepository = centreRepository;
            _masterDataAllocationRepository = masterDataAllocationRepository;
            _commoditySupplierRepository = commoditySupplierRepository;
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListCommodityProducers(Guid? CommoditySupplierId, Boolean? showInactive, string srchParam, int page = 1, int itemsperpage = 10)
        {
            if (CommoditySupplierId != null)
            {
                ViewBag.CommoditySupplierId = CommoditySupplierId;
                commoditySupplier = CommoditySupplierId.Value;
            }
            try
            {
                if (itemsperpage != null)
                {
                    ViewModelBase.ItemsPerPage = itemsperpage;
                }
                bool showinactive = true;
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
                int skip = take*currentPageIndex;

                var query = new QueryCommodityProducer(){ShowInactive = showinactive, SupplierId = commoditySupplier, Name = srchParam, Skip = skip};
                
                var ls = _commodityProducerViewModelBuilder.Query(query);
                var data = ls.Data;
                var count = ls.Count;

                var supplier =_commoditySupplierRepository.GetById(commoditySupplier) as CommoditySupplier;
                if (supplier != null) ViewBag.CommoditySupplierType = supplier.CommoditySupplierType;
                
                return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage));
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list commodity types " + ex.Message);
                _log.Error("Failed to list commodity types" + ex.ToString());
                return View();
            }
        }
        
        [HttpPost]
        public ActionResult ListCommodityProducers(Guid? CommoditySupplierId, bool? showInactive, int? page, string srch, string srchParam, int? itemsperpage)
        {
            if (CommoditySupplierId != null)
            {
                ViewBag.CommoditySupplierId = CommoditySupplierId;
            }
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
                var ls = _commodityProducerViewModelBuilder.SearchCommodityProducers(srchParam, showinactive);
                int currentPageIndex = page.HasValue ? page.Value - 1 : 0;

                var supplier = _commoditySupplierRepository.GetById(commoditySupplier) as CommoditySupplier;
                if (supplier != null) ViewBag.CommoditySupplierType = supplier.CommoditySupplierType;


                if (command == "Search")
                {
                    return View(ls.ToPagedList(currentPageIndex,  ViewModelBase.ItemsPerPage));
                }
                else
                {
                    return RedirectToAction("ListCommodityProducers", new { showInactive = showInactive, srch = "Search", srchParam = "", CommoditySupplierId = CommoditySupplierId });
                }

            }
            catch (Exception ex)
            {

                ViewBag.msg = ex.Message;
                return View();
            }

        }
        
        public ActionResult DetailsCommodityProducer(Guid id)
        {
            try
            {
                CommodityProducerViewModel commidityTypeVM = _commodityProducerViewModelBuilder.Get(id);
                return View(commidityTypeVM);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult EditCommodityProducer(Guid commoditySupplierId, Guid id, string msg)
        {
            try
            {

                CommodityProducerViewModel model = _commodityProducerViewModelBuilder.Get(id);
                LoadUnassignedCenters(model.Id,model);
                _commodityProducerViewModelBuilder.SetUp(LoadUnassignedCenters(model.Id, model));
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditCommodityProducer(Guid commoditySupplierId, CommodityProducerViewModel vm)
        {
            var path = Request.Path;
            if (path.Split('/').LastOrDefault() == "AssignCentre")
            {
                return AssignCentre(vm);
            }

            vm.CommoditySupplierId = commoditySupplierId;
            _commodityProducerViewModelBuilder.SetUp(vm);
            try
            {
                _commodityProducerViewModelBuilder.Save(vm);
                TempData["msg"] = "Commodity producer Successfully Edited";
                return RedirectToAction("ListCommodityProducers", new { CommoditySupplierId = commoditySupplierId });
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to edit Commodity producer " + ve.Message);
                _log.Error("Failed to edit Commodity producer" + ve.ToString());

                _commodityProducerViewModelBuilder.SetUp(vm);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to edit Commodity producer " + ex.Message);
                _log.Error("Failed to edit Commodity producer" + ex.ToString());
                _commodityProducerViewModelBuilder.SetUp(vm);
                return View();
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateCommodityProducer(Guid CommoditySupplierId)
        {
            var model = new CommodityProducerViewModel();
            model.CommoditySupplierId = CommoditySupplierId;
            _commodityProducerViewModelBuilder.SetUp(model);

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateCommodityProducer(Guid CommoditySupplierId, CommodityProducerViewModel vm)
        {
            try
            {
                vm.Id = Guid.NewGuid();
                vm.CommoditySupplierId = CommoditySupplierId;
                _commodityProducerViewModelBuilder.Save(vm);

                TempData["msg"] = "Farm Successfully Created";

                return RedirectToAction("ListCommodityProducers", new { CommoditySupplierId = CommoditySupplierId });
            }
            catch (DomainValidationException ve)
            {
                TempData["msg"] = ve.Message;
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to create commodity producers " + ve.Message);
                _log.Error("Failed to create commodity producers" + ve.ToString());

                _commodityProducerViewModelBuilder.SetUp(vm);
                return View(vm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to create Commodity producers " + ex.Message);
                _log.Error("Failed to create Commodity producers" + ex.ToString());

                _commodityProducerViewModelBuilder.SetUp(vm);
                return View(vm);
            }

        }
        
        public void SetInactive(Guid Id)
        {
            _commodityProducerViewModelBuilder.SetInactive(Id);

        }

        public void SetActive(Guid Id)
        {
            _commodityProducerViewModelBuilder.SetActive(Id);

        }
        
        public ActionResult Deactivate(Guid CommoditySupplierId, Guid id)
        {
            try
            {
                _commodityProducerViewModelBuilder.SetInactive(id);

                TempData["msg"] = "Farm Successfully Deactivated";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate commodity producer " + ex.Message);
                _log.Error("Failed to deactivate commodity producer" + ex.ToString());
            }
            return RedirectToAction("ListCommodityProducers", new { CommoditySupplierId = CommoditySupplierId });
        }

        public ActionResult Activate(Guid CommoditySupplierId, Guid id)
        {
            try
            {
                _commodityProducerViewModelBuilder.SetActive(id);
                TempData["msg"] = "Farm Successfully activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate commodity type" + ex.Message);
                _log.Error("Failed to activate commodity type" + ex.ToString());


            }

            return RedirectToAction("ListCommodityProducers", new { CommoditySupplierId = CommoditySupplierId });
        }


        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult DeleteCommodityProducer(Guid CommoditySupplierId, Guid id)
        {
            try
            {
                _commodityProducerViewModelBuilder.SetAsDeleted(id);
                TempData["msg"] = "Farm Successfully deleted";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate commodity producer" + ex.Message);
                _log.Error("Failed to activate commodity producer" + ex.ToString());


            }

            return RedirectToAction("ListCommodityProducers", new { CommoditySupplierId = CommoditySupplierId });
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult AssignCentre(CommodityProducerViewModel vm)
        {
            try
            {
                var center = _centreRepository.GetById(vm.SelectedCentreId);
                vm.AssignedFarmCentres=new List<Centre>(){center};
                CommodityProducerViewModel pvm = _commodityProducerViewModelBuilder.AssignCentre(vm);
                LoadUnassignedCenters(vm.Id,vm);
                var msg = "Route successfully added";
                return RedirectToAction("EditCommodityProducer", new { commoditySupplierId = pvm.CommoditySupplierId, id = pvm.Id, msg });
            }
            catch(DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                
                var existing = _commodityProducerViewModelBuilder.Get(vm.Id);
                if (existing == null)
                    return View("CreateCommodityProducer", vm);

                return View("EditCommodityProducer", vm);
                
            }catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to create centres " + ex.Message);

                var existing = _commodityProducerViewModelBuilder.Get(vm.Id);
                if (existing == null)
                    return View("CreateCommodityProducer", vm);

                return View("EditCommodityProducer", vm);
            }
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        private CommodityProducerViewModel LoadUnassignedCenters(Guid producerId,CommodityProducerViewModel vm)
        {
            var unassignedCenters = _centreRepository.GetAll().ToList();
            var assignedAllocations = _masterDataAllocationRepository.GetByAllocationType(MasterDataAllocationType.CommodityProducerCentreAllocation, producerId);
            var assignedCenters = new List<Centre>();
            foreach (var assignedAllocation in assignedAllocations)
            {
               var center = _centreRepository.GetById(assignedAllocation.EntityBId);
                if(center!=null)
                {
                    assignedCenters.Add(center);
                    if (unassignedCenters.Any(l => l == center))
                    {
                        unassignedCenters.Remove(center);
                    } 
                }
            }
            vm.AssignedFarmCentres.Clear();
            vm.AssignedFarmCentres = assignedCenters;
            ViewBag.UnassignedCenters = unassignedCenters.Select(r => new { r.Id, r.Name }).ToDictionary(d => d.Id, d => d.Name);
            return vm;
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult UnAssignCentre(Guid centreid, Guid farmid, Guid commoditySupplierId)
        {

            try
            {
                _commodityProducerViewModelBuilder.UnAssignCentre(centreid, farmid);
                var msg = "Route successfully added";
                return RedirectToAction("EditCommodityProducer", new {commoditySupplierId, id = farmid, msg });
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                var vm = _commodityProducerViewModelBuilder.Get(farmid);

                var existing = _commodityProducerViewModelBuilder.Get(vm.Id);
                if (existing == null)
                    return View("CreateCommodityProducer", vm);

                return View("EditCommodityProducer", vm);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to create centres " + ex.Message);

                var vm = _commodityProducerViewModelBuilder.Get(farmid);

                var existing = _commodityProducerViewModelBuilder.Get(vm.Id);
                if (existing == null)
                    return View("CreateCommodityProducer", vm);

                return View("EditCommodityProducer", vm);
            }
        }

        public ActionResult GetRegionCentres(Guid regionId, Guid farmerId)//
        {
            object items = null;
            return Json(items, JsonRequestBehavior.AllowGet);
        }
    }

}

