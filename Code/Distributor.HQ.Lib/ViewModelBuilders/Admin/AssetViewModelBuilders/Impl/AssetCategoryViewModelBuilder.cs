using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Repository.Master.AssetRepositories;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.AssetViewModel;
using Distributr.Core.Repository.Master.CoolerTypeRepositories;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.AssetViewModelBuilders.Impl
{
    public class AssetCategoryViewModelBuilder : IAssetCategoryViewModelBuilder
    {       
        IAssetCategoryRepository _assetCategoryRepository;
        IAssetTypeRepository _assetTypeRepository;

        public AssetCategoryViewModelBuilder(IAssetCategoryRepository assetCategoryRepository, IAssetTypeRepository assetTypeRepository)
        {
            _assetCategoryRepository = assetCategoryRepository;
            _assetTypeRepository = assetTypeRepository;
        }

        public IList<AssetCategoryViewModel> GetAll(bool inactive = false)
        {
            var assetCategory = _assetCategoryRepository.GetAll(inactive);
            return assetCategory
                .Select(n => new AssetCategoryViewModel
                {
                    Id = n.Id,
                    Name = n.Name,
                    Description = n.Description,
                    AssetTypeId = n.AssetType.Id,
                    AssetTypeName = n.AssetType.Name,
                    IsActive = n._Status==EntityStatus.Active?true:false
                }
                ).ToList();
        }

        public AssetCategoryViewModel Get(Guid id)
        {
            AssetCategory assetCategory = _assetCategoryRepository.GetById(id);
            if (assetCategory == null) return null;
               
            return Map(assetCategory);
        }

        public void Save(AssetCategoryViewModel assetCategoryViewModel)
        {
            AssetCategory assetCat = new AssetCategory(assetCategoryViewModel.Id)
            {
                Name = assetCategoryViewModel.Name,
                Description=assetCategoryViewModel.Description,
                AssetType = _assetTypeRepository.GetById(assetCategoryViewModel.AssetTypeId)
            };
            _assetCategoryRepository.Save(assetCat);
        }


        private AssetCategoryViewModel Map(AssetCategory assetCategory)
        {
            AssetCategoryViewModel assetCategoryViewModel = new AssetCategoryViewModel();
            assetCategoryViewModel.Id = assetCategory.Id;
            assetCategoryViewModel.Name = assetCategory.Name;
            assetCategoryViewModel.Description = assetCategory.Description;
            if (assetCategory.AssetType != null)
               {
                   assetCategoryViewModel.AssetTypeId = _assetTypeRepository.GetById(assetCategory.AssetType.Id).Id;
               }
            if (assetCategory.AssetType != null)
               {
                   assetCategoryViewModel.AssetTypeName = _assetTypeRepository.GetById(assetCategory.AssetType.Id).Name;
               }
            if (assetCategory._Status == EntityStatus.Active)
                assetCategoryViewModel.IsActive = true;
               return assetCategoryViewModel;
            
        }
        public Dictionary<Guid, string> AssetType()
        {
            return _assetTypeRepository.GetAll().OrderBy(n => n.Name)
                .Select(r => new { r.Id, r.Name }).ToList().ToDictionary(d => d.Id, d => d.Name);
        }

        public QueryResult<AssetCategoryViewModel> Query(QueryStandard query)
        {
            var queryResult = _assetCategoryRepository.Query(query);

            var result = new QueryResult<AssetCategoryViewModel>();

            result.Data = queryResult.Data.Select(Map).ToList();
            result.Count = queryResult.Count;

            return result;
        }

        public IList<AssetCategoryViewModel> GetByAssetType(Guid assetTypeId, bool inactive = false)
        {
            var assetCategories = _assetCategoryRepository.GetAll(inactive).Where(n => n.AssetType.Id == assetTypeId);
            return assetCategories
                .Select(n => new AssetCategoryViewModel
                {
                    Id = n.Id,
                    Name = n.Name,
                    Description = n.Description,
                    AssetTypeId = n.AssetType.Id,
                    AssetTypeName = n.AssetType.Name,
                    IsActive = n._Status == EntityStatus.Active ? true : false
                }
                ).ToList();
        }

        public void SetInactive(Guid id)
        {
            AssetCategory outCategory = _assetCategoryRepository.GetById(id);
            _assetCategoryRepository.SetInactive(outCategory);
        }

        public void SetDeleted(Guid id)
        {
            AssetCategory outCategory = _assetCategoryRepository.GetById(id);
            _assetCategoryRepository.SetAsDeleted(outCategory);
        }

        public IList<AssetCategoryViewModel> Search(string searchParam, bool inactive = false)
        {
            var assetCategory = _assetCategoryRepository.GetAll(inactive).Where(n=>n.Name.ToLower().StartsWith(searchParam.ToLower()));
            return assetCategory
                .Select(n => new AssetCategoryViewModel
                {
                    Id = n.Id,
                    Name = n.Name,
                    Description = n.Description,
                    IsActive = n._Status == EntityStatus.Active ? true : false
                }
                ).ToList();
        }


        public void SetActive(Guid id)
        {
            AssetCategory assetCategory = _assetCategoryRepository.GetById(id);
            _assetCategoryRepository.SetActive(assetCategory);
        }
    }
}
