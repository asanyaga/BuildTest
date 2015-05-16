using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModels.Admin.AssetViewModel;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CommodityViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CommodityViewModelBuilders.Impl
{
    public class CommodityTypeViewModelBuilder: ICommodityTypeViewModelBuilder
    {
        ICommodityTypeRepository _commodityTypeRepository;
        private IRegionRepository _regionRepository;
        private IMasterDataUsage _masterDataUsage;

        public CommodityTypeViewModelBuilder(ICommodityTypeRepository commodityTypeRepository, IRegionRepository regionRepository, IMasterDataUsage masterDataUsage)
        {
            _commodityTypeRepository = commodityTypeRepository;
            _regionRepository = regionRepository;
            _masterDataUsage = masterDataUsage;
        }

        #region Implementation of ICommodityTypeViewModelBuilder

        public IList<CommodityTypeViewModel> GetAll(bool inactive = false)
        {
            var productTypes = _commodityTypeRepository.GetAll(inactive).Select(s => Map(s)).ToList();
            return productTypes;
        }

        public List<CommodityTypeViewModel> SearchCommodityTypes(string srchParam, bool inactive = false)
        {
            var items = _commodityTypeRepository.GetAll(inactive).OfType<CommodityType>().Where(n => (n.Name.ToLower().StartsWith(srchParam.ToLower())));
            return items.Select(n => Map(n)).ToList();
        }

        CommodityTypeViewModel Map(CommodityType commodityType)
        {


            CommodityTypeViewModel commodityTypeView = new CommodityTypeViewModel();
            commodityTypeView.Id = commodityType.Id;
            commodityTypeView.Code = commodityType.Code;
            commodityTypeView.Description = commodityType.Description;
            commodityTypeView.Name = commodityType.Name;
            commodityTypeView.IsActive = (int) commodityType._Status;

            return commodityTypeView;
        }
        public Dictionary<Guid, string> Region()
        {
            return _regionRepository.GetAll().OrderBy(n => n.Name)
                .Select(r => new { r.Id, r.Name }).ToList().ToDictionary(d => d.Id, d => d.Name);
        }

    

        public QueryResult<CommodityTypeViewModel> Query(QueryStandard query)
        {
            var queryResult = _commodityTypeRepository.Query(query);
            var results = new QueryResult<CommodityTypeViewModel>();
            results.Count = queryResult.Count;
            results.Data = queryResult.Data.Select(Map).ToList();

            return results;

        }

        public IList<CommodityTypeViewModel> QueryList(QueryResult result)
        {
            //var result= _commodityTypeRepository.Query(query);
            return result.Data.OfType<CommodityType>().ToList().Select(Map).ToList();
        }


        public CommodityTypeViewModel Get(Guid id)
        {
            CommodityType commodityType = _commodityTypeRepository.GetById(id);

            if (commodityType == null) return null;

            return Map(commodityType);
        }

        public void Save(CommodityTypeViewModel commodityTypeViewmodel)
        {
            CommodityType commodityType = new CommodityType(commodityTypeViewmodel.Id);
            commodityType.Name = commodityTypeViewmodel.Name;
            commodityType.Description = commodityTypeViewmodel.Description;
            commodityType.Code = commodityTypeViewmodel.Code;
            commodityType._Status = EntityStatus.Active;
            _commodityTypeRepository.Save(commodityType);
        }

        

        public void SetInactive(Guid Id)
        {

            CommodityType commodityType = _commodityTypeRepository.GetById(Id);
            if (_masterDataUsage.CheckCommodtiyTypeIsUsed(commodityType, EntityStatus.Inactive))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot deactivate commodity type. Commodity type is assigned to commodity(s). Remove dependencies first to continue");
            }
            _commodityTypeRepository.SetInactive(commodityType);
        }

        public void SetActive(Guid Id)
        {
            CommodityType hub = _commodityTypeRepository.GetById(Id);
            _commodityTypeRepository.SetActive(hub);
        }

        public void SetAsDeleted(Guid Id)
        {
            CommodityType commodityType = _commodityTypeRepository.GetById(Id);
            if (_masterDataUsage.CheckCommodtiyTypeIsUsed(commodityType, EntityStatus.Deleted))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot delete commodity type. Commodity type is assigned to commodity(s). Remove dependencies first to continue");
            }
            _commodityTypeRepository.SetAsDeleted(commodityType);
        }

        #endregion
    }
}
