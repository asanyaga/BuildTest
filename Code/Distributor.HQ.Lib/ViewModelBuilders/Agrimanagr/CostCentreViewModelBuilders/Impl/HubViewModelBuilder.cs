using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CostCentreViewModelBuilders.Impl
{
    public class HubViewModelBuilder : IHubViewModelBuilder
    {
        private IRegionRepository _regionRepository;
        private IContactRepository _contactRepository;
        private IProducerRepository _producerRepository;
        private IHubRepository _hubRepository;
        private IMasterDataUsage _masterDataUsage;
        private ICommodityProducerRepository _commodityProducerRepository;


        public HubViewModelBuilder(IRegionRepository regionRepository, IContactRepository contactRepository,
                                   IProducerRepository producerRepository, IHubRepository hubRepository,
                                   ICommodityProducerRepository commodityProducerRepository, IMasterDataUsage masterDataUsage)
        {
            _regionRepository = regionRepository;
            _contactRepository = contactRepository;
            _producerRepository = producerRepository;
            _hubRepository = hubRepository;
            _commodityProducerRepository = commodityProducerRepository;
            _masterDataUsage = masterDataUsage;
        }

        public IList<HubViewModel> GetAll(bool inactive = false)
        {
            var hubs = _hubRepository.GetAll(inactive).Select(n => Map((Hub) n)).ToList();
            return hubs;
        }

        private HubViewModel Map(Hub hub)
        {
            HubViewModel hubVM = new HubViewModel();
            hubVM.Id = hub.Id;
            hubVM.CostCentreCode = hub.CostCentreCode;
            hubVM.RegionId = hub.Region.Id;
            hubVM.RegionName = hub.Region.Name;
            hubVM.Name = hub.Name;
            hubVM.VatRegistrationNo = hub.VatRegistrationNo;
            hubVM.Longitude = hub.Longitude;
            hubVM.Latitude = hub.Latitude;
            hubVM.ParentCostCentreId = hub.ParentCostCentre.Id;
            hubVM.IsActive = (int) hub._Status;
            hubVM.CanEditHubRegion = _masterDataUsage.CanEditHubOrDistributrRegion(hub);
            return hubVM;
        }

        public List<HubViewModel> SearchHubs(string srchParam, bool inactive = false)
        {
            var items =
                _hubRepository.GetAll(inactive).OfType<Hub>().Where(
                    n => (n.Name.ToLower().StartsWith(srchParam.ToLower())));
            return items.Select(n => Map(n)).ToList();
        }

        public HubViewModel Get(Guid id)
        {
            Hub hub = (Hub) _hubRepository.GetById(id);

            if (hub == null) return null;
            return Map(hub);
        }

        public void Save(HubViewModel hubViewModel)
        {
            Hub hub = new Hub(hubViewModel.Id);
            hub.Name = hubViewModel.Name;
            hub.CostCentreCode = hubViewModel.CostCentreCode;
            hub.Region = _regionRepository.GetById(hubViewModel.RegionId);
            hub.VatRegistrationNo = hubViewModel.VatRegistrationNo;
            hub.Longitude = hubViewModel.Longitude;
            hub.Region = _regionRepository.GetById(hubViewModel.RegionId);
            hub.Latitude = hubViewModel.Latitude;
            hub._Status = EntityStatus.Active;
            hub.ParentCostCentre = new CostCentreRef {Id = hubViewModel.ParentCostCentreId};
            hub.CostCentreType = CostCentreType.Hub;
            _hubRepository.Save(hub);
        }

        public void SetInactive(Guid Id)
        {
            CostCentre hub = _hubRepository.GetById(Id);

            if (_masterDataUsage.CheckHubIsUsed(hub as Hub, EntityStatus.Inactive))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot deactivate hub. Hub is assigned to commodity producer(s) and/or purchasing clerk cost centres. Remove dependencies first to continue");
            }
            _hubRepository.SetInactive(hub);
        }

        public void SetActive(Guid Id)
        {
            CostCentre hub = _hubRepository.GetById(Id);
            _hubRepository.SetActive(hub);
        }

        public void SetAsDeleted(Guid Id)
        {
            CostCentre hub = _hubRepository.GetById(Id);

            if (_masterDataUsage.CheckHubIsUsed(hub as Hub, EntityStatus.Deleted))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot delete hub. Hub is assigned to commodity producer(s) and/or purchasing clerk cost centres. Remove dependencies first to continue");
            }
            _hubRepository.SetAsDeleted(hub);
        }

        public Dictionary<Guid, string> Region()
        {
            return _regionRepository.GetAll().OrderBy(n => n.Name)
                .Select(r => new {r.Id, r.Name}).ToList().ToDictionary(d => d.Id, d => d.Name);
        }

        public Dictionary<Guid, string> Contact()
        {
            return _contactRepository.GetAll().OrderBy(n => n.Firstname)
                .Select(r => new {r.Id, r.Firstname}).ToList().ToDictionary(d => d.Id, d => d.Firstname);
        }

        public Dictionary<Guid, string> ParentCostCentre()
        {
            return _producerRepository.GetAll().OrderBy(n => n.Name).Where(
                n => n.CostCentreType == CostCentreType.Producer)
                .Select(r => new {r.Id, r.Name}).ToList().ToDictionary(d => d.Id, d => d.Name);
        }

      
        public QueryResult<HubViewModel> Query(QueryStandard query)
        {
            var queryResult = _hubRepository.Query(query);
            var results = new QueryResult<HubViewModel>();
            results.Count = queryResult.Count;
            results.Data = queryResult.Data.Select(Map).ToList();

            return results;
        }

        public IList<HubViewModel> Querylist(QueryResult result)
        {
            return result.Data.OfType<Hub>().ToList().Select(Map).ToList();
        }
    }
}
