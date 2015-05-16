using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.MasterDataAllocationRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModels.Admin.AssetViewModel;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CostCentreViewModelBuilders.Impl
{
    public class CommodityProducerViewModelBuilder: ICommodityProducerViewModelBuilder
    {
        private ICommodityProducerRepository _commodityProducerRepository;
        private IRegionRepository _regionRepository;
        private ICentreRepository _centreRepository;
        private IMasterDataUsage _masterDataUsage;
        
        private ICommoditySupplierRepository _commoditySupplierRepository;
        private IMasterDataAllocationRepository _masterDataAllocationRepository;

        public CommodityProducerViewModelBuilder(ICommoditySupplierRepository commoditySupplierRepository, IRegionRepository regionRepository, ICommodityProducerRepository commodityProducerRepository, ICentreRepository centreRepository, IMasterDataAllocationRepository masterDataAllocationRepository, IMasterDataUsage masterDataUsage)
        {
            _commoditySupplierRepository = commoditySupplierRepository;
            _regionRepository = regionRepository;
            _commodityProducerRepository = commodityProducerRepository;
            _centreRepository = centreRepository;
            _masterDataAllocationRepository = masterDataAllocationRepository;
            _masterDataUsage = masterDataUsage;
        }

        public void SetUp(CommodityProducerViewModel vm)
        {
            if(vm.AssignedFarmCentres == null)
                vm.AssignedFarmCentres = new List<Centre>();
            vm.SelectedCentreId = Guid.Empty;
            vm.UnAsignedCentresList = GetUnAssignedCentres(vm);
        }

        public SelectList GetUnAssignedCentres(CommodityProducerViewModel vm)
        {
            List<SelectListItem> selectList = new List<SelectListItem>
                                                  {
                                                      new SelectListItem
                                                          {
                                                              Selected = true,
                                                              Text = "-- Select a centre --",
                                                              Value = Guid.Empty.ToString()
                                                          }
                                                  };

            var assignedCentres = new List<Centre>();
            if(vm.AssignedFarmCentres != null && vm.AssignedFarmCentres.Any())
            {
                assignedCentres.AddRange(vm.AssignedFarmCentres);
            }
            if(vm.HubId == Guid.Empty)
            {
                CommoditySupplier supplier = _commoditySupplierRepository.GetById(vm.CommoditySupplierId) as CommoditySupplier;
                if (supplier != null)
                    vm.HubId = supplier.ParentCostCentre.Id;
            }
            var centers = _centreRepository.GetByHubId(vm.HubId);
                            

            if(centers!=null)
            {
                centers.Where(n => assignedCentres.Select(p => p.Id).All(q => q != n.Id))
                            .OrderBy(n => n.Name)
                            .ToList().ForEach(n => selectList.Add(new SelectListItem() { Text = n.Name, Value = n.Id.ToString() }));
            }
                

           return new SelectList(selectList, "Value", "Text");
        }

        public IList<CommodityProducerViewModel> GetAll(bool inactive = false)
        {
            var commodityProducers = _commodityProducerRepository.GetAll(inactive).Select(s => Map(s)).ToList();
            return commodityProducers;
        }
        
        public CommodityProducerViewModel AssignCentre(CommodityProducerViewModel vm)
        {
            if(vm.Id == Guid.Empty)
            {
                vm.Id = Guid.NewGuid();
                Save(vm);
            }
            if (_commodityProducerRepository.GetById(vm.Id) != null)
            {
                MasterDataAllocation allocation = new MasterDataAllocation(Guid.NewGuid())
                                                      {
                                                          AllocationType =
                                                              MasterDataAllocationType.CommodityProducerCentreAllocation,
                                                          EntityAId = vm.Id,
                                                          EntityBId = vm.SelectedCentreId
                                                      };
                _masterDataAllocationRepository.Save(allocation);
                Save(vm);
            }
            return vm;
        }

        public void UnAssignCentre(Guid centreId, Guid farmId)
        {
            var allocation = _masterDataAllocationRepository.GetByAllocationType(MasterDataAllocationType.CommodityProducerCentreAllocation,farmId, centreId);
            if (allocation != null && allocation.Count > 0)
            {
                var masterDataAllocation = allocation.FirstOrDefault();
                if (masterDataAllocation != null)
                    _masterDataAllocationRepository.DeleteAllocation(masterDataAllocation.Id);
            }
        }

        public QueryResult<CommodityProducerViewModel> Query(QueryCommodityProducer q)
        {
            var queryResult = _commodityProducerRepository.Query(q);

            var result = new QueryResult<CommodityProducerViewModel>();

            result.Data = queryResult.Data.Select(Map).ToList();
            result.Count = queryResult.Count;

            return result;
        }

        CommodityProducerViewModel Map(CommodityProducer commodityProducer)
        {
            CommodityProducerViewModel commodityProducerVM = new CommodityProducerViewModel
                                                                 {
                                                                     Id = commodityProducer.Id,
                                                                     Code = commodityProducer.Code,
                                                                     Description = commodityProducer.Description,
                                                                     Name = commodityProducer.Name,
                                                                     Acrage = commodityProducer.Acrage,
                                                                     RegNo = commodityProducer.RegNo,
                                                                     PhysicalAddress = commodityProducer.PhysicalAddress
                                                                 };

            commodityProducerVM.Description = commodityProducer.Description;
            commodityProducerVM.CommoditySupplierId = commodityProducer.CommoditySupplier.Id;
            commodityProducerVM.IsActive = (int)commodityProducer._Status;
            commodityProducerVM.AssignedFarmCentres = commodityProducer.CommodityProducerCentres;
            commodityProducerVM.HubId = commodityProducer.CommoditySupplier.ParentCostCentre.Id;

            return commodityProducerVM;
        }

        public List<CommodityProducerViewModel> SearchCommodityProducers(string srchParam, bool inactive = false)
        {
            var items = _commodityProducerRepository.GetAll(inactive).Where(n => (n.Name.ToLower().Contains(srchParam.ToLower())));
            return items.Select(n => Map(n)).ToList();
        }

        public CommodityProducerViewModel Get(Guid id)
        {
            CommodityProducer commodityProducer = _commodityProducerRepository.GetById(id);

            if (commodityProducer == null) return null;

            return Map(commodityProducer);
        }

        public void Save(CommodityProducerViewModel vm)
        {
            CommodityProducer commodityProducer = new CommodityProducer(vm.Id);
            commodityProducer.Name = vm.Name;
            commodityProducer.Description = vm.Description;
            commodityProducer.Code = vm.Code;
            commodityProducer.Acrage = vm.Acrage;
            commodityProducer.RegNo = vm.RegNo;
            commodityProducer.PhysicalAddress = vm.PhysicalAddress;
            commodityProducer.CommoditySupplier =  _commoditySupplierRepository.GetById(vm.CommoditySupplierId) as CommoditySupplier;
            commodityProducer.CommodityProducerCentres = vm.AssignedFarmCentres;

            _commodityProducerRepository.Save(commodityProducer); //.Save(commodityProducer);
        }

        public void SetInactive(Guid Id)
        {
            CommodityProducer commodityProducer = _commodityProducerRepository.GetById(Id);
            if (_masterDataUsage.CommodityProducerHasPurchases(commodityProducer))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot deactivate commodity producer. Commodity producer is used in transaction(s). Remove dependencies first to continue");
            }
            _commodityProducerRepository.SetInactive(commodityProducer);
        }

        public void SetActive(Guid Id)
        {
            CommodityProducer commodityProducer = _commodityProducerRepository.GetById(Id);
            _commodityProducerRepository.SetActive(commodityProducer);
        }

        public void SetAsDeleted(Guid Id)
        {
            CommodityProducer commodityProducer = _commodityProducerRepository.GetById(Id);
            if (_masterDataUsage.CommodityProducerHasPurchases(commodityProducer))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot deactivate commodity producer. Commodity producer is used in transaction(s). Remove dependencies first to continue");
            }
            _commodityProducerRepository.SetAsDeleted(commodityProducer);
        }

    }
}
