using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CostCentreViewModelBuilders.Impl
{
    public class StoreViewModelBuilder: IStoreViewModelBuilder
    {
        private IStoreRepository _storeRepository;
        private IHubRepository _hubRepository;

        public StoreViewModelBuilder(IStoreRepository storeRepository, IHubRepository hubRepository)
        {
            _storeRepository = storeRepository;
            _hubRepository = hubRepository;
        }

        #region Implementation of IStoreViewModelBuilder

        public IList<StoreViewModel> GetAll(bool inactive = false)
        {
            var stores = _storeRepository.GetAll(true).Where(n => n.CostCentreType == CostCentreType.Store).Select(n => Map((Store)n)).ToList();
            return stores;
        }
        StoreViewModel Map(Store store)
        {
            StoreViewModel storeViewModel = new StoreViewModel();
            storeViewModel.Id = store.Id;
            storeViewModel.Code = store.CostCentreCode;
            storeViewModel.Name = store.Name;
            storeViewModel.VatRegistrationNo = store.VatRegistrationNo;
            storeViewModel.Longitude = store.Longitude;
            storeViewModel.Latitude = store.Latitude;
            storeViewModel.ParentCostCentreId = store.ParentCostCentre.Id;
            storeViewModel.ParentCostCentreName = _hubRepository.GetById(store.ParentCostCentre.Id).Name;
            storeViewModel.IsActive = (int)store._Status;
            return storeViewModel;
        }

        public List<StoreViewModel> SearchStores(string srchParam, bool inactive = false)
        {
            var items = _storeRepository.GetAll(inactive).OfType<Store>().Where(n => (n.Name.ToLower().StartsWith(srchParam.ToLower())));
            return items.Select(n => Map(n)).ToList();
        }

        public StoreViewModel Get(Guid id)
        {
            Store store = (Store)_storeRepository.GetById(id);
            if (store == null) return null;
            return Map(store);
        }

        public void Save(StoreViewModel storeViewModel)
        {
            Guid Id = Guid.NewGuid();
            if(storeViewModel.Id != null)
            {
                Id = storeViewModel.Id;
            }
            Store store = new Store(Id);
            store.Name = storeViewModel.Name;
            store.CostCentreCode = storeViewModel.Code;
            store.VatRegistrationNo = storeViewModel.VatRegistrationNo;
            store.Longitude = storeViewModel.Longitude;
            store.Latitude = storeViewModel.Latitude;
            store._Status = EntityStatus.Active;
            store.ParentCostCentre = new CostCentreRef { Id = storeViewModel.ParentCostCentreId };// _producerRepository.GetById();
            store.CostCentreType = CostCentreType.Store;
            _storeRepository.Save(store);
        }

        public void SetInactive(Guid Id)
        {
           
            Store store = (Store) _storeRepository.GetById(Id);
            _storeRepository.SetInactive(store);
        }

        public void SetActive(Guid Id)
        {
            CostCentre store = _storeRepository.GetById(Id);
            _storeRepository.SetActive(store);
        }

        public void SetAsDeleted(Guid Id)
        {
           
            CostCentre hub = _storeRepository.GetById(Id);
            _storeRepository.SetAsDeleted(hub);
        }

        public Dictionary<Guid, string> Contact()
        {
            throw new NotImplementedException();
        }

        public Dictionary<Guid, string> ParentCostCentre()
        {
            return _hubRepository.GetAll().OrderBy(n => n.Name).Where(n => n.CostCentreType == CostCentreType.Hub)
                 .Select(r => new { r.Id, r.Name }).ToList().ToDictionary(d => d.Id, d => d.Name);
        }

        public Dictionary<int, string> CostCentreTypes()
        {
            var dict = Enum.GetValues(typeof(CostCentreType))
               .Cast<CostCentreType>()
               .Where(n => (int)n == (int)CostCentreType.Store)
               .ToDictionary(t => (int)t, t => t.ToString());
            return dict;
        }

        public QueryResult<StoreViewModel> Query(QueryStandard query)
        {
            var queryResult = _storeRepository.Query(query);

            var result = new QueryResult<StoreViewModel>();

            result.Data = queryResult.Data.Select(Map).ToList();
            result.Count = queryResult.Count;

            return result;
        }

        #endregion
    }
}
