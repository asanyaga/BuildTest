using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.BankEntities;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CommodityDTOs;
using Distributr.Core.Repository.Master.BankRepositories;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Mapping;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CostCentreViewModelBuilders.Impl
{
    public class CommoditySupplierViewModelBuilder : ICommoditySupplierViewModelBuilder
    {
        private ICommoditySupplierRepository _commoditySupplierRepository;
        private IProducerRepository _producerRepository;
        private IBankRepository _bankRepository;
        private IBankBranchRepository _bankBranchRepository;
        private ICentreRepository _centreRepository;
        private ICommodityProducerRepository _commodityProducerRepository;
        private ICommodityOwnerRepository _commodityOwnerRepository;
        private IMasterDataUsage _masterDataUsage;
        private IMasterDataToDTOMapping _mastertodto;

        public CommoditySupplierViewModelBuilder(ICommoditySupplierRepository commoditySupplierRepository,
                                                 IProducerRepository producerRepository,
                                                 ICommodityProducerRepository commodityProducerRepository,
                                                 ICommodityOwnerRepository commodityOwnerRepository, IMasterDataUsage masterDataUsage, IBankRepository bankRepository, IBankBranchRepository bankBranchRepository, IMasterDataToDTOMapping mastertodto, ICentreRepository centreRepository)
        {
            _commoditySupplierRepository = commoditySupplierRepository;
            _producerRepository = producerRepository;
            _commodityProducerRepository = commodityProducerRepository;
            _commodityOwnerRepository = commodityOwnerRepository;
            _masterDataUsage = masterDataUsage;
            _bankRepository = bankRepository;
            _bankBranchRepository = bankBranchRepository;
            _mastertodto = mastertodto;
            _centreRepository = centreRepository;
        }

        public IList<CommoditySupplierListingViewModel> GetAll(bool inactive = false)
        {
            var hubs = _commoditySupplierRepository.GetAll(true).Select(n => Map((CommoditySupplier)n)).ToList();
            
            //var hubs = _hubRepository.GetAll(inactive).Select(s => Map((Hub) s)).Where(s=>s.CostCentreType == CostCentreType.Hub).ToList();
            return hubs;
        }

         public QueryResult<CommoditySupplierListingViewModel> Query(QueryCommoditySupplier query)
         {
             var queryResult = _commoditySupplierRepository.Query(query);

             var result = new QueryResult<CommoditySupplierListingViewModel>();

             result.Data = queryResult.Data.Select(Map).ToList();
             result.Count = queryResult.Count;

             return result;
         }

        private CommoditySupplierListingViewModel Map(CommoditySupplier commoditySupplier)
        {
            var bank = _bankRepository.GetById(commoditySupplier.BankId) ?? new Bank(Guid.NewGuid());
            var bankBranch = _bankBranchRepository.GetById(commoditySupplier.BankBranchId) ?? new BankBranch(Guid.NewGuid());

            var commoditySupplierVM = new CommoditySupplierListingViewModel();
            commoditySupplierVM.Id = commoditySupplier.Id;
            commoditySupplierVM.CostCentreCode = commoditySupplier.CostCentreCode;
            commoditySupplierVM.CommoditySupplierType = (int)commoditySupplier.CommoditySupplierType;
            commoditySupplierVM.Name = commoditySupplier.Name;
            commoditySupplierVM.JoinDate = commoditySupplier.JoinDate;
            commoditySupplierVM.AccountNo = commoditySupplier.AccountNo;
            commoditySupplierVM.PinNo = commoditySupplier.PinNo;
            commoditySupplierVM.Bank = bank.Name??"";
            commoditySupplierVM.BankBranch= bankBranch.Name??"";
            commoditySupplierVM.ParentCostCentreId = commoditySupplier.ParentCostCentre.Id;
            commoditySupplierVM.IsActive = (int)commoditySupplier._Status;
            return commoditySupplierVM;
        }

        public List<CommoditySupplierListingViewModel> SearchCommoditySuppliers(string srchParam, bool inactive = false)
        {
            var items =
                _commoditySupplierRepository.GetAll(inactive).OfType<CommoditySupplier>().Where(
                    n => (n.Name.ToLower().StartsWith(srchParam.ToLower())));
            return items.Select(n => Map(n)).ToList();
        }

        public CommoditySupplierListingViewModel Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public CommoditySupplierListingViewModel Edit(Guid id)
        {
            CommoditySupplier commoditySupplier = (CommoditySupplier)_commoditySupplierRepository.GetById(id);
            if (commoditySupplier == null) return null;
            return Map(commoditySupplier);
        }

        public CommoditySupplierDTO GetDto(Guid id)
        {
            CommoditySupplier commoditySupplier = (CommoditySupplier)_commoditySupplierRepository.GetById(id);
            if (commoditySupplier == null) return null;
            return _mastertodto.Map(commoditySupplier); //Map(commoditySupplier);
        }

        public void Save(CommoditySupplierDTO model)
        {
            CommoditySupplier commoditySupplier = new CommoditySupplier(model.MasterId);
            commoditySupplier.AccountName = model.Name;
            commoditySupplier.Name = model.Name;
            commoditySupplier.CostCentreCode = model.CostCentreCode;
            commoditySupplier.CommoditySupplierType =
                (CommoditySupplierType)model.CommoditySupplierTypeId;
            commoditySupplier.JoinDate = DateTime.Now;
            commoditySupplier.AccountNo = model.AccountNo;
            commoditySupplier.PinNo = model.PinNo;
            commoditySupplier.BankId = model.BankId;
            commoditySupplier.BankBranchId = model.BankBranchId;
            commoditySupplier._Status = EntityStatus.Active;
            commoditySupplier.ParentCostCentre = new CostCentreRef { Id = model.ParentCostCentreId };
            commoditySupplier.CostCentreType = CostCentreType.CommoditySupplier;
            _commoditySupplierRepository.Save(commoditySupplier);
        }

        public void SetInactive(Guid Id)
        {
            var supplier = _commoditySupplierRepository.GetById(Id) as CommoditySupplier;
            if (_masterDataUsage.CheckCommoditySupplierIsUsed(supplier, EntityStatus.Inactive))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot deactivate commodity supplier. Commodity producer is used in transaction(s) and/or has been assigned commodity producer(s) and/or commodity owner(s). Remove dependencies first to continue");
            }
            _commoditySupplierRepository.SetInactive(supplier);
        }

        public void SetActive(Guid Id)
        {
            CostCentre commoditySupplier = _commoditySupplierRepository.GetById(Id);
            _commoditySupplierRepository.SetActive(commoditySupplier);
        }

        public void SetAsDeleted(Guid Id)
        {
            var supplier = _commoditySupplierRepository.GetById(Id) as CommoditySupplier;
            if (_masterDataUsage.CheckCommoditySupplierIsUsed(supplier, EntityStatus.Deleted))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot delete commodity supplier. Commodity producer is used in transaction(s) and/or has been assigned commodity producer(s) and/or commodity owner(s). Remove dependencies first to continue");
            }
            _commoditySupplierRepository.SetAsDeleted(supplier);
        }

        public Dictionary<int, string> CommoditySupplierType()
        {
            var dict = Enum.GetValues(typeof(CommoditySupplierType))
                .Cast<CommoditySupplierType>()
                .ToDictionary(t => (int)t, t => t.ToString());
            return dict;
        }

        public Dictionary<Guid, string> ParentCostCentre()
        {
            return _producerRepository.GetAll().OrderBy(n => n.Name).Where(n => n.CostCentreType == CostCentreType.Hub)
                .Select(r => new { r.Id, r.Name }).ToList().ToDictionary(d => d.Id, d => d.Name);
        }

        public Dictionary<Guid, string> GetBanks()
        {
            return _bankRepository.GetAll().OrderBy(n => n.Name).Select(n => new {n.Id, n.Name}).ToList().ToDictionary(n => n.Id, n => n.Name);
        }

        public List<BankBranch> GetBankBranches(Guid bankId)
        {
            return _bankBranchRepository.GetByBankMasterId(bankId);
        }

        public void SetUp(CommoditySupplierViewModel vm)
        {
            if (vm.AssignedFarmCentres == null)
                vm.AssignedFarmCentres = new List<Centre>();
            vm.UnAsignedCentresList = GetUnAssignedCentres(vm);
        }

        public SelectList GetUnAssignedCentres(CommoditySupplierViewModel vm)
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
            if (vm.AssignedFarmCentres != null && vm.AssignedFarmCentres.Any())
            {
                assignedCentres.AddRange(vm.AssignedFarmCentres);
            }
            if (vm.HubId == Guid.Empty)
            {
                CommoditySupplier supplier = _commoditySupplierRepository.GetById(vm.CommoditySupplierId) as CommoditySupplier;
                if (supplier != null)
                    vm.HubId = supplier.ParentCostCentre.Id;
            }
            _centreRepository.GetByHubId(vm.HubId)
                .Where(n => assignedCentres.Select(p => p.Id).All(q => q != n.Id))
                .OrderBy(n => n.Name)
                .ToList().ForEach(n => selectList.Add(new SelectListItem() { Text = n.Name, Value = n.Id.ToString() }));

            return new SelectList(selectList, "Value", "Text");
        }

        public void SetUpNew(CommoditySupplierViewModel vm)
        {
            if (vm.AssignedFarmCentres == null)
                vm.AssignedFarmCentres = new List<Centre>();
            vm.UnAsignedCentresList = GetAllCentres(vm);
        }

       
        
        public SelectList GetAllCentres(CommoditySupplierViewModel vm)
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
            if (vm.AssignedFarmCentres != null && vm.AssignedFarmCentres.Any())
            {
                assignedCentres.AddRange(vm.AssignedFarmCentres);
            }
            if (vm.HubId == Guid.Empty)
            {
                CommoditySupplier supplier = _commoditySupplierRepository.GetById(vm.CommoditySupplierId) as CommoditySupplier;
                if (supplier != null)
                    vm.HubId = supplier.ParentCostCentre.Id;
            }
            _centreRepository.GetAll()
                .OrderBy(n => n.Name)
                .ToList().ForEach(n => selectList.Add(new SelectListItem() { Text = n.Name, Value = n.Id.ToString() }));

            return new SelectList(selectList, "Value", "Text");
        }
    }
}
