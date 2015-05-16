using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.MasterDataAllocationRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.Paging;
using Distributr.HQ.Lib.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CostCentreViewModelBuilders;
using Distributr.HQ.Lib.ViewModels;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using log4net;

namespace Agrimanagr.HQ.Areas.Agrimanagr.Controllers
{
    public class CommoditySupplierController : Controller
    {
        //
        // GET: /Agrimanagr/CommoditySupplier/
        private ICommoditySupplierViewModelBuilder _commoditySupplierViewModelBuilder;
        private ICommodityOwnerViewModelBuilder _commodityOwnerViewModelBuilder;
        private ICommodityProducerViewModelBuilder _commodityProducerViewModelBuilder;
        private ICommoditySupplierRepository _commoditysupplierRepository;
        private ICentreRepository _centreRepository;
        private IBankBranchRepository _bankBranchRepository;
        private readonly IMasterDataAllocationRepository _masterDataAllocationRepository;
        private IList<Centre> _assignedCentresList;
        protected static readonly ILog _log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CommoditySupplierController(ICommoditySupplierViewModelBuilder commoditySupplierViewModelBuilder, IBankBranchRepository bankBranchRepository, ICommoditySupplierRepository commoditysupplierRepository, ICommodityOwnerViewModelBuilder commodityOwnerViewModelBuilder, ICommodityProducerViewModelBuilder commodityProducerViewModelBuilder, IList<Centre> assignedCentresList, ICentreRepository centreRepository, IMasterDataAllocationRepository masterDataAllocationRepository)
        {
            _commoditySupplierViewModelBuilder = commoditySupplierViewModelBuilder;
            _bankBranchRepository = bankBranchRepository;
            _commoditysupplierRepository = commoditysupplierRepository;
            _commodityOwnerViewModelBuilder = commodityOwnerViewModelBuilder;
            _commodityProducerViewModelBuilder = commodityProducerViewModelBuilder;
            _assignedCentresList = assignedCentresList;
            _centreRepository = centreRepository;
            _masterDataAllocationRepository = masterDataAllocationRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult ListCommoditySuppliers(Boolean? showInactive, int page = 1, int itemsperpage = 10, string srchParam = "")
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
                ViewBag.srchParam = srchParam;
                int currentPageIndex = page < 0 ? 0 : page - 1;
                int take = itemsperpage;
                int skip = take*currentPageIndex;
                var query=new QueryCommoditySupplier(){ShowInactive = showinactive,Name = srchParam, Skip = skip, Take = take};

                var ls = _commoditySupplierViewModelBuilder.Query(query);
                var data = ls.Data;
                var count = ls.Count;

               return View(data.ToPagedList(currentPageIndex, ViewModelBase.ItemsPerPage, count));
            }
            catch (Exception ex)
            {
                _log.Debug("Failed to list hubs " + ex.Message);
                _log.Error("Failed to list hubs" + ex.ToString());
                return View();
            }
        }

        public ActionResult Deactivate(Guid id)
        {
            try
            {
                _commoditySupplierViewModelBuilder.SetInactive(id);
                TempData["msg"] = "Commodity supplier Successfully Deactivated";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to deactivate Commodity supplier " + ex.Message);
                _log.Error("Failed to deactivate Commodity supplier" + ex.ToString());
            }
            return RedirectToAction("ListCommoditySuppliers");
        }

        public ActionResult Activate(Guid id)
        {
            try
            {
                _commoditySupplierViewModelBuilder.SetActive(id);
                TempData["msg"] = "Commodity supplier Successfully activated";
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to activate Commodity supplier" + ex.Message);
                _log.Error("Failed to activate Commodity supplier" + ex.ToString());

            }

            return RedirectToAction("ListCommoditySuppliers");
        }

        [Authorize(Roles = "RoleViewMasterData")]
        public ActionResult DetailsCommoditySupplier(Guid id)
        {
            try
            {
                CommoditySupplierListingViewModel commoditySupplierVM = _commoditySupplierViewModelBuilder.Edit(id);
                return View(commoditySupplierVM);
            }
            catch (Exception ex)
            {

                ViewBag.msg = ex.Message;
                return View();
            }

        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult EditCommoditySupplier(Guid id)
        {
            ViewBag.CommoditySupplierTypeList = _commoditySupplierViewModelBuilder.CommoditySupplierType();
            ViewBag.ParentCostCentreList = _commoditySupplierViewModelBuilder.ParentCostCentre();
            ViewBag.Banks = _commoditySupplierViewModelBuilder.GetBanks();
            try
            {

                CommoditySupplierDTO commoditySupplierViewModel = _commoditySupplierViewModelBuilder.GetDto(id);
                LoadBranches(commoditySupplierViewModel.BankId);
                return View(commoditySupplierViewModel);

                //var commoditySupplierViewModel = _commoditySupplierViewModelBuilder.GetDto(id);
            }
            catch (Exception ex)
            {
                ViewBag.msg = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public ActionResult EditCommoditySupplier(CommoditySupplierDTO vm)
        {
            ViewBag.CommoditySupplierTypeList = _commoditySupplierViewModelBuilder.CommoditySupplierType();
            ViewBag.ParentCostCentreList = _commoditySupplierViewModelBuilder.ParentCostCentre();
            ViewBag.Banks = _commoditySupplierViewModelBuilder.GetBanks();
            try
            {
                LoadBranches(vm.BankId);
                _commoditySupplierViewModelBuilder.Save(vm);
                TempData["msg"] = "Commodity supplier Successfully Edited";
                return RedirectToAction("ListCommoditySuppliers");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to edit Commodity producer " + ve.Message);
                _log.Error("Failed to edit Commodity producer" + ve.ToString());

                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to edit Commodity producer " + ex.Message);
                _log.Error("Failed to edit Commodity producer" + ex.ToString());
                return View();
            }
        }

        [Authorize(Roles = "RoleAddMasterData")]
        public ActionResult CreateCommoditySupplier()
        {
            ViewBag.Banks = _commoditySupplierViewModelBuilder.GetBanks();
            ViewBag.CommoditySupplierTypeList = _commoditySupplierViewModelBuilder.CommoditySupplierType();
            ViewBag.ParentCostCentreList = _commoditySupplierViewModelBuilder.ParentCostCentre();
            ViewBag.GenderList = _commodityOwnerViewModelBuilder.Gender();
            ViewBag.MaritalStatusList = _commodityOwnerViewModelBuilder.MaritalStatus();
            ViewBag.CommodityOwnerTypeList = _commodityOwnerViewModelBuilder.CommodityOwnerType();
            ViewBag.UnassignedCentresList = _centreRepository.GetAll().Select(r => new { r.Id, r.Name }).ToDictionary(d => d.Id, d => d.Name);

            var model = new CommoditySupplierViewModel();
            model.CommoditySupplierId = Guid.NewGuid();
            model.CommodityOwnerId = Guid.NewGuid();
            model.CommodityProducerId = Guid.NewGuid();
            _commoditySupplierViewModelBuilder.SetUpNew(model);
            return View(model);
        }

        public JsonResult LoadBankBranch(Guid? bankId)
        {
            if (bankId == null || bankId == Guid.Empty)
                return Json(new SelectListItem {Text = "Select Branch", Value = Guid.Empty.ToString()},
                            JsonRequestBehavior.AllowGet);

            var branches =_bankBranchRepository.GetByBankMasterId(bankId.Value).OrderBy(n=>n.Name).Select(n => new SelectListItem() {Value = n.Id.ToString(), Text = n.Name});
            return Json(branches, JsonRequestBehavior.AllowGet);
        }

        public void AssignCenter(Guid centerId, Guid commodityProducerId)
        {
            var allocation = new MasterDataAllocation(Guid.NewGuid())
            {
                AllocationType = MasterDataAllocationType.CommodityProducerCentreAllocation,
                EntityAId = commodityProducerId,
                EntityBId = centerId
            };
            _masterDataAllocationRepository.Save(allocation);
        }


        [HttpPost]
        public ActionResult CreateCommoditySupplier(CommoditySupplierViewModel vm)
        {
            
            ViewBag.CommoditySupplierTypeList = _commoditySupplierViewModelBuilder.CommoditySupplierType();
            ViewBag.ParentCostCentreList = _commoditySupplierViewModelBuilder.ParentCostCentre();
            ViewBag.Banks = _commoditySupplierViewModelBuilder.GetBanks();
            ViewBag.GenderList = _commodityOwnerViewModelBuilder.Gender();
            ViewBag.MaritalStatusList = _commodityOwnerViewModelBuilder.MaritalStatus();
            ViewBag.CommodityOwnerTypeList = _commodityOwnerViewModelBuilder.CommodityOwnerType();
            ViewBag.UnassignedCentresList = _centreRepository.GetAll().Select(r => new { r.Id, r.Name }).ToDictionary(d => d.Id, d => d.Name);
            LoadBranches(vm.BankId);

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var allocatedCenter = _centreRepository.GetById(vm.SelectedCentreId);
            try
            {
                using (var tran = new TransactionScope())
                {
                    var commoditySupplierDto = new CommoditySupplierDTO
                    {
                            MasterId = vm.CommoditySupplierId,
                            Name = vm.Name,
                            AccountName = vm.AccountName,
                            AccountNo = vm.AccountNo,
                            PinNo = vm.PinNo,
                            BankId = vm.BankId,
                            BankBranchId = vm.BankBranchId,
                            CommoditySupplierTypeId = vm.CommoditySupplierType,
                            CostCentreCode = vm.CostCentreCode,
                            ParentCostCentreId = vm.ParentCostCentreId,
                        
                        };
                    _commoditySupplierViewModelBuilder.Save(commoditySupplierDto);
                    var commodityOwnerViewModel = new CommodityOwnerViewModel
                        {
                            Id=vm.CommodityOwnerId,
                            Code=vm.OwnerCode,
                            CommodityOwnerType=vm.CommodityOwnerType,
                            DateOfBirth=vm.DateOfBirth,
                            BusinessNumber=vm.BusinessNumber,
                            Description=vm.Description,
                            Email=vm.Email,
                            FaxNumber=vm.FaxNumber,
                            FirstName=vm.FirstName,
                            LastName=vm.LastName,
                            Surname=vm.Surname,
                            Gender=vm.Gender,
                            IdNo=vm.IdNo,
                            MaritalStatus=vm.MaritalStatus,
                            OfficeNumber=vm.OfficeNumber,
                            PhoneNumber=vm.PhoneNumber,
                            PhysicalAddress=vm.PhysicalAddress,
                            PinNo=vm.OwnerPinNo,
                            PostalAddress=vm.PostalAddress,
                            CommoditySupplier=vm.CommoditySupplierId
                        };

                    _commodityOwnerViewModelBuilder.Save(commodityOwnerViewModel);
                    var commodityProducerViewModel = new CommodityProducerViewModel
                        {
                            Id=vm.CommodityProducerId,
                            Code=vm.FarmCode,
                            Acrage=vm.Acrage,
                            Name=vm.FarmName,
                            RegNo=vm.RegNo,
                            PhysicalAddress=vm.FarmPhysicalAddress,
                            Description=vm.FarmDescription,
                            CommoditySupplierId=vm.CommoditySupplierId,
                            AssignedFarmCentres = new List<Centre>(){allocatedCenter},

                        };
                    _commodityProducerViewModelBuilder.Save(commodityProducerViewModel);

                    AssignCenter(vm.SelectedCentreId, vm.CommodityProducerId);

                    TempData["msg"] = "Commodity supplier Successfully Created";
                    tran.Complete();
                }


                return RedirectToAction("ListCommoditySuppliers");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                _log.Debug("Failed to create commodity suppliers " + ve.Message);
                _log.Error("Failed to create commodity suppliers" + ve.ToString());

                return View(vm);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to create Commodity suppliers " + ex.Message);
                _log.Error("Failed to create Commodity suppliers" + ex.ToString());

                return View(vm);
            }

        }

        [Authorize(Roles = "RoleDeleteMasterData")]
        public ActionResult DeleteCommoditySupplier(Guid id)
        {
            try
            {
                _commoditySupplierViewModelBuilder.SetAsDeleted(id);
                TempData["msg"] = "Commodity supplier Successfully deleted";
            }
            catch (DomainValidationException dve)
            {
                ValidationSummary.DomainValidationErrors(dve, ModelState);
                TempData["msg"] = dve.Message;
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
                _log.Debug("Failed to delete Commodity supplier" + ex.Message);
                _log.Error("Failed to delete Commodity supplier" + ex.ToString());


            }

            return RedirectToAction("ListCommoditySuppliers");
        }

        private void LoadBranches(Guid bankid)
        {
            if (bankid != Guid.Empty)
            {
                ViewBag.BankBranches = _bankBranchRepository.GetByBankMasterId(bankid).Select(n => new { n.Id, n.Name }).OrderBy(n => n.Name).ToDictionary(p => p.Id, p => p.Name);

            }
        }

        private List<Centre> LoadAssignedCenters(Guid commodityProducerId,Guid selectedCentreId)
        {
            var allocation = new MasterDataAllocation(Guid.NewGuid())
                {
                    AllocationType = MasterDataAllocationType.CommodityProducerCentreAllocation,
                    EntityAId = commodityProducerId,
                    EntityBId = selectedCentreId
                };


                _masterDataAllocationRepository.Save(allocation);
                var assignedMasterDataAllocation =
                    _masterDataAllocationRepository.GetByAllocationType(
                        MasterDataAllocationType.CommodityProducerCentreAllocation, commodityProducerId);

                var unassignedCenters = _centreRepository.GetAll().ToList();

                foreach (var c in assignedMasterDataAllocation)
                {
                    var center = _centreRepository.GetById(c.EntityBId);

                    _assignedCentresList.Add(center);
                    if (center != null && unassignedCenters.Any(l => l == center))
                    {
                        unassignedCenters.Remove(center);
                    }
                }
            
            ViewBag.UnassignedCentresList = unassignedCenters.Select(r => new { r.Id, r.Name }).ToDictionary(d => d.Id, d => d.Name);

            //if (assignedFarmCentres == null)
            //{
                List<Centre> assignedFarmCentres = new List<Centre>();
               assignedFarmCentres.AddRange(_assignedCentresList);

            return assignedFarmCentres;
        }

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult AssignCentre(CommoditySupplierViewModel m)
        {

            ViewBag.CommoditySupplierTypeList = _commoditySupplierViewModelBuilder.CommoditySupplierType();
            ViewBag.ParentCostCentreList = _commoditySupplierViewModelBuilder.ParentCostCentre();
            ViewBag.Banks = _commoditySupplierViewModelBuilder.GetBanks();
            ViewBag.GenderList = _commodityOwnerViewModelBuilder.Gender();
            ViewBag.MaritalStatusList = _commodityOwnerViewModelBuilder.MaritalStatus();
            ViewBag.CommodityOwnerTypeList = _commodityOwnerViewModelBuilder.CommodityOwnerType();

            if (m != null)
            {
                LoadBranches(m.BankId);

             //  m.AssignedFarmCentres = LoadAssignedCenters(m.CommodityProducerId, m.SelectedCentreId);

                return View("CreateCommoditySupplier", m);
            }
            return View("CreateCommoditySupplier");
        }

        //public Action UnAssignCentre(Centre m)
        //{
        //    _assignedCentresList.Add(m);

        //}

        [Authorize(Roles = "RoleUpdateMasterData")]
        public ActionResult UnAssignCentre(Guid commodityProducerId,Guid centerId,Guid bankId)//Guid centreid, Guid farmid, Guid commoditySupplierId)
        {
            ViewBag.CommoditySupplierTypeList = _commoditySupplierViewModelBuilder.CommoditySupplierType();
            ViewBag.ParentCostCentreList = _commoditySupplierViewModelBuilder.ParentCostCentre();
            ViewBag.Banks = _commoditySupplierViewModelBuilder.GetBanks();
            ViewBag.GenderList = _commodityOwnerViewModelBuilder.Gender();
            ViewBag.MaritalStatusList = _commodityOwnerViewModelBuilder.MaritalStatus();
            ViewBag.CommodityOwnerTypeList = _commodityOwnerViewModelBuilder.CommodityOwnerType();

            LoadBankBranch(bankId);

            try
            {
                _commodityProducerViewModelBuilder.UnAssignCentre(centerId,commodityProducerId);
               // ViewBag.AssignedFarmCenters = LoadAssignedCenters(commodityProducerId);
                var msg = "Center Successfully Unassigned";
                return View("CreateCommoditySupplier");
            }
            catch (DomainValidationException ve)
            {
                ValidationSummary.DomainValidationErrors(ve, ModelState);
                var vm = _commodityProducerViewModelBuilder.Get(commodityProducerId);

                var existing = _commodityProducerViewModelBuilder.Get(vm.Id);
                if (existing == null)
                    return View("CreateCommoditySupplier");

                return View("EditCommoditySupplier");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _log.Debug("Failed to create centres " + ex.Message);

                var vm = _commodityProducerViewModelBuilder.Get(commodityProducerId);

                var existing = _commodityProducerViewModelBuilder.Get(vm.Id);
                if (existing == null)
                    return View("CreateCommoditySupplier");

                return View("EditCommoditySupplier");
            }
        }

        public ActionResult GetRegionCentres(Guid regionId, Guid farmerId)//
        {
            object items = null;
            return Json(items, JsonRequestBehavior.AllowGet);
        }

    }
}
