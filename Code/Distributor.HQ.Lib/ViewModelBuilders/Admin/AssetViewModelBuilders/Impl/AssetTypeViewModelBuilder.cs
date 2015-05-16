using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.AssetViewModel;
using Distributr.Core.Repository.Master.CoolerTypeRepositories;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.AssetViewModelBuilders.Impl
{
    public class AssetTypeViewModelBuilder : IAssetTypeViewModelBuilder
    {
        IAssetTypeRepository _assetTypeRepository;
        
        public AssetTypeViewModelBuilder(IAssetTypeRepository assetTypeRepository) 
        {
            _assetTypeRepository = assetTypeRepository;
        }
        
        AssetTypeViewModel Map(AssetType assetType) 
        {
            return new AssetTypeViewModel { Id = assetType.Id, Name = assetType.Name, Description = assetType.Description, IsActive = assetType._Status == EntityStatus.Active ? true : false };
        }
        
        
        public IList<AssetTypeViewModel> GetAll(bool inactive = false)
        {
            var assetType = _assetTypeRepository.GetAll(inactive);
            return assetType.Select(n => Map(n)).ToList();
        }

        public List<AssetTypeViewModel> Search(string srchParam, bool inactive = false)
        {
            var items = _assetTypeRepository.GetAll().Where(n => (n.Name.ToLower().StartsWith(srchParam.ToLower()) || (n.Description.ToLower().StartsWith(srchParam.ToLower()))));
            return items.Select(n => Map(n)).ToList();
        }

        public AssetTypeViewModel Get(Guid Id)
        {
            AssetType assetType = _assetTypeRepository.GetById(Id);
            if (assetType == null) return null;
            return Map(assetType);
        }

        public void Save(AssetTypeViewModel assetTypeViewModel)
        {
            AssetType assetType = new AssetType(assetTypeViewModel.Id)
            {
                Name=assetTypeViewModel.Name,
                Description=assetTypeViewModel.Description
            };
            _assetTypeRepository.Save(assetType);
        }

        public void SetInactive(Guid id)
        {
            AssetType assetType = _assetTypeRepository.GetById(id);
            _assetTypeRepository.SetInactive(assetType);
        }


        public void SetActive(Guid Id)
        {
            AssetType assetType = _assetTypeRepository.GetById(Id);
            _assetTypeRepository.SetActive(assetType);
        }

        public void SetDeleted(Guid Id)
        {
            AssetType assetType = _assetTypeRepository.GetById(Id);
            _assetTypeRepository.SetAsDeleted(assetType);
        }

        public QueryResult<AssetTypeViewModel> Query(QueryStandard query)
        {
            var result = _assetTypeRepository.Query(query);
            
            var queryResult = new QueryResult<AssetTypeViewModel>();

            queryResult.Data = result.Data.Select(Map).ToList();
            queryResult.Count = result.Count;

            return queryResult;
        }
    }
}

