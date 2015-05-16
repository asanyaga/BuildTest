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
using Distributr.Core.Utility.Validation;
using Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CostCentreViewModelBuilders;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.CostCentreViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.CostCentreViewModelBuilders.Impl
{
    public class CommodityOwnerTypeViewModelBuilder: ICommodityOwnerTypeViewModelBuilder
    {
        private ICommodityOwnerTypeRepository _commodityOwnerTypeRepository;
        private IMasterDataUsage _masterDataUsage;

        public CommodityOwnerTypeViewModelBuilder(ICommodityOwnerTypeRepository commodityOwnerTypeRepository, IMasterDataUsage masterDataUsage)
        {
            _commodityOwnerTypeRepository = commodityOwnerTypeRepository;
            _masterDataUsage = masterDataUsage;
        }
        CommodityOwnerTypeViewModel Map(CommodityOwnerType commodityOwnerType)
        {
            return new CommodityOwnerTypeViewModel { Id = commodityOwnerType.Id, Name = commodityOwnerType.Name,Code = commodityOwnerType.Code,Description = commodityOwnerType.Description, IsActive = commodityOwnerType._Status == EntityStatus.Active ? true : false };
        }
        

        public IList<CommodityOwnerTypeViewModel> GetAll(bool inactive = false)
        {
            var commodityOwnerType = _commodityOwnerTypeRepository.GetAll(inactive);
            return commodityOwnerType.Select(n => Map(n)).ToList();
        }

        public List<CommodityOwnerTypeViewModel> SearchCommodityOwnerTypes(string srchParam, bool inactive = false)
        {
            var items = _commodityOwnerTypeRepository.GetAll().Where(n => (n.Name.ToLower().StartsWith(srchParam.ToLower()) || (n.Description.ToLower().StartsWith(srchParam.ToLower()))));
            return items.Select(n => Map(n)).ToList();
        }

        public CommodityOwnerTypeViewModel Get(Guid id)
        {
            CommodityOwnerType commodityOwnerType = _commodityOwnerTypeRepository.GetById(id);
            if (commodityOwnerType == null) return null;
            return Map(commodityOwnerType);
        }

        public void Save(CommodityOwnerTypeViewModel commodityOwnerTypeViewmodel)
        {
            CommodityOwnerType commodityOwnerType = new CommodityOwnerType(commodityOwnerTypeViewmodel.Id)
            {
                Name = commodityOwnerTypeViewmodel.Name,
                Description = commodityOwnerTypeViewmodel.Description,
                Code = commodityOwnerTypeViewmodel.Code
            };
            _commodityOwnerTypeRepository.Save(commodityOwnerType);
        }

        public void SetInactive(Guid Id)
        {
            CommodityOwnerType assetType = _commodityOwnerTypeRepository.GetById(Id);
            if (_masterDataUsage.CheckCommodityOwnerTypeIsUsed(assetType, EntityStatus.Inactive))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot deactivate commodity owner type. Commodity owner type is assigned to commodity owner(s). Remove dependencies first to continue");
            }
            _commodityOwnerTypeRepository.SetInactive(assetType);
        }

        public void SetActive(Guid Id)
        {
            CommodityOwnerType assetType = _commodityOwnerTypeRepository.GetById(Id);
            _commodityOwnerTypeRepository.SetActive(assetType);
        }

        public void SetAsDeleted(Guid Id)
        {
            CommodityOwnerType assetType = _commodityOwnerTypeRepository.GetById(Id);
            if (_masterDataUsage.CheckCommodityOwnerTypeIsUsed(assetType, EntityStatus.Inactive))
            {
                throw new DomainValidationException(new ValidationResultInfo(),
                    "Cannot deactivate commodity owner type. Commodity owner type is assigned to commodity owner(s). Remove dependencies first to continue");
            }
            _commodityOwnerTypeRepository.SetAsDeleted(assetType);
        }

        public QueryResult<CommodityOwnerTypeViewModel> Query(QueryStandard query)
        {
            var queryResult = _commodityOwnerTypeRepository.Query(query);

            var result = new QueryResult<CommodityOwnerTypeViewModel>();
            result.Data = queryResult.Data.Select(Map).ToList();
            result.Count = queryResult.Count;

            return result;
        }

        public IList<CommodityOwnerTypeViewModel> QueryList(QueryResult result)
        {
            return result.Data.OfType<CommodityOwnerType>().ToList().Select(Map).ToList();
        }
    }
}
