using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.AssetViewModel;
using Distributr.Core.Repository.Master.AssetRepositories;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Repository.Master.CoolerTypeRepositories;
using Distributr.Core.Utility.MasterData;


namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.AssetViewModelBuilders.Impl
{
    public class AssetViewModelBuilder : IAssetViewModelBuilder
    {
        IAssetTypeRepository _assetTypeRepository;
        IAssetStatusRepository _assetStatusRepository;
        IAssetRepository _assetRepository;
        IAssetCategoryRepository _assetCategoryRepository;
        public AssetViewModelBuilder(IAssetRepository assetRepository, IAssetTypeRepository assetTypeRepository, IAssetCategoryRepository assetCategoryRepository, IAssetStatusRepository assetStatusRepository)
        {
            _assetRepository = assetRepository;
            _assetCategoryRepository = assetCategoryRepository;
            _assetTypeRepository = assetTypeRepository;
            _assetStatusRepository = assetStatusRepository;
        }        

        public IList<AssetViewModel> GetAll(bool inactive = false) 
        { 
            var assets= _assetRepository.GetAll(inactive).Select(s=>Map(s)).ToList();
          return assets;
        }

        public List<AssetViewModel> SearchAssets(string srchParam, bool inactive = false) 
        {
            var items = _assetRepository.GetAll().Where(n => (n.Name.ToLower().StartsWith(srchParam.ToLower()) || (n.Code.ToLower().StartsWith(srchParam.ToLower()))));
            return items.Select(n => Map(n)).ToList();
        }

        public AssetViewModel Get(Guid id)
        {
            Asset asset = _assetRepository.GetById(id);

            if (asset == null) return null;

            return Map(asset);
        }

        public void Save(AssetViewModel assetVM) 
        {
            Asset asset = new Asset(assetVM.Id);
            asset.Name = assetVM.Name;
            asset.Capacity = assetVM.Capacity;
            asset.Code = assetVM.Code;
            asset.SerialNo = assetVM.SerialNo;
            asset.AssetNo = assetVM.AssetNo;
            asset.AssetType = _assetTypeRepository.GetById(assetVM.AssetTypeId); 
            asset.AssetStatus = _assetStatusRepository.GetById(assetVM.AssetStatusId);
            asset.AssetCategory = _assetCategoryRepository.GetById(assetVM.AssetCategoryId);
            _assetRepository.Save(asset);
        }

        public void SetInactive(Guid Id) 
        {
            Asset asset = _assetRepository.GetById(Id);
            _assetRepository.SetInactive(asset);
        }

        public Dictionary<Guid, string> AssetType() 
        {
            return _assetTypeRepository.GetAll().ToList().OrderBy(a => a.Name)
                .Select(a => new { a.Id, a.Name }).ToDictionary(a => a.Id, a => a.Name);
        }
        public Dictionary<Guid, string> AssetStatus()
        {
            return _assetStatusRepository.GetAll().ToList().OrderBy(a => a.Name)
                .Select(a => new { a.Id, a.Name }).ToDictionary(a => a.Id, a => a.Name);
        }
        public Dictionary<Guid, string> AssetCategory() 
        {
            return _assetCategoryRepository.GetAll().ToList().OrderBy(a => a.Name)
                .Select(a => new { a.Id, a.Name }).ToDictionary(a => a.Id, a => a.Name);
        }

        public QueryResult<AssetViewModel> Query(QueryStandard query)
        {
            var result = _assetRepository.Query(query);

            var queryResult = new QueryResult<AssetViewModel>();

            queryResult.Data = result.Data.Select(Map).ToList();
            queryResult.Count = result.Count;

            return queryResult;
        }

        AssetViewModel Map(Asset asset) 
        {
            var assetTypes = _assetTypeRepository.GetAll().ToDictionary(n => n.Id, n => n.Name);
            var assetStatus = _assetStatusRepository.GetAll().ToDictionary(n => n.Id, n => n.Name);
            var assetCategories = _assetCategoryRepository.GetAll().ToDictionary(n => n.Id, n => n.Name);
           
            AssetViewModel assetVM = new AssetViewModel();
            assetVM.Id = asset.Id;
            assetVM.Name = asset.Name;
            assetVM.Capacity = asset.Capacity;
            assetVM.Code = asset.Code;
            assetVM.Name = asset.Name;
            assetVM.SerialNo = asset.SerialNo;
            assetVM.AssetNo = asset.AssetNo;
            assetVM.IsActive = asset._Status == EntityStatus.Active ? true : false;

            if (asset.AssetStatus != null)
            {
                assetVM.AssetStatusId = _assetStatusRepository.GetById(asset.AssetStatus.Id).Id;
                assetVM.AssetStatus = _assetStatusRepository.GetById(asset.AssetStatus.Id).Name; 
            }

            if (asset.AssetType != null)
            {
                assetVM.AssetTypeId = _assetTypeRepository.GetById(asset.AssetType.Id).Id;
                assetVM.AssetTypeName = _assetTypeRepository.GetById(asset.AssetType.Id).Name; 
            }

            if (asset.AssetCategory != null)
            {
                assetVM.AssetCategoryId = _assetCategoryRepository.GetById(asset.AssetCategory.Id).Id;
                assetVM.AssetCategoryName = _assetCategoryRepository.GetById(asset.AssetCategory.Id).Name;
            }    
            return assetVM;
        }


        public void SetActive(Guid Id)
        {
            Asset asset = _assetRepository.GetById(Id);
            _assetRepository.SetActive(asset);
        }

        public void SetAsDeleted(Guid Id)
        {
            Asset asset = _assetRepository.GetById(Id);
            _assetRepository.SetAsDeleted(asset);
        }
    }
}
