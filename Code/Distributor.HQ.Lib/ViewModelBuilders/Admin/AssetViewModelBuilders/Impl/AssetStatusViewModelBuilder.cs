using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.AssetRepositories;
using Distributr.Core.Utility;
using Distributr.HQ.Lib.ViewModels.Admin.AssetViewModel;
using Distributr.Core.Domain.Master.AssetEntities;
using Distributr.Core.Utility.MasterData;

namespace Distributr.HQ.Lib.ViewModelBuilders.Admin.AssetViewModelBuilders.Impl
{
    public class AssetStatusViewModelBuilder : IAssetStatusViewModelBuilder
    {
        IAssetStatusRepository _assetStatusRepository;
       public AssetStatusViewModelBuilder(IAssetStatusRepository assetStatusRepository)
       {
           _assetStatusRepository = assetStatusRepository;
       }
       public void Save(AssetStatusViewModel aStatusVM)
       {
            AssetStatus aStatus = new AssetStatus(aStatusVM.Id)
            {
                Name=aStatusVM.Name,
                Description=aStatusVM.Description,                
            };
            _assetStatusRepository.Save(aStatus);
        }

        public AssetStatusViewModel GetById(Guid Id)
        {
            AssetStatus aStatus = _assetStatusRepository.GetById(Id);
            if (aStatus == null) return null;
               
            return Map(aStatus);
        }

        public List<AssetStatusViewModel> GetAll(bool inactive = false)
        {
            return _assetStatusRepository.GetAll(inactive).Select(m=>Map(m)).ToList();
        }

        public void SetInactive(Guid Id)
        {
            AssetStatus aStatus = _assetStatusRepository.GetById(Id);
            _assetStatusRepository.SetInactive(aStatus);
        }
        AssetStatusViewModel Map(AssetStatus assetStatus)
        {
            return new AssetStatusViewModel
            {
                Name=assetStatus.Name,           
                Id=assetStatus.Id,
                 IsActive = assetStatus._Status==EntityStatus.Active?true:false  ,    
                Description=assetStatus.Description
            };
        }

        public List<AssetStatusViewModel> Search(string srcParam, bool inactive = false)
        {
            return _assetStatusRepository.GetAll(inactive).Where(m=>(m.Name.ToLower().StartsWith(srcParam.ToLower()))||(m.Name.ToLower().StartsWith(srcParam.ToLower()))||(m.Description.ToLower().StartsWith(srcParam.ToLower()))).Select(m => Map(m)).ToList();
        }


        public void SetActive(Guid Id)
        {
            AssetStatus assetStatus = _assetStatusRepository.GetById(Id);
            _assetStatusRepository.SetActive(assetStatus);
        }

        public void SetDeleted(Guid Id)
        {
            AssetStatus aStatus = _assetStatusRepository.GetById(Id);
            _assetStatusRepository.SetAsDeleted(aStatus);
        }

        public QueryResult<AssetStatusViewModel> Query(QueryStandard q)
        {
            var result = _assetStatusRepository.Query(q);

            var queryResult = new QueryResult<AssetStatusViewModel>();

            queryResult.Count = result.Count;
            queryResult.Data = result.Data.Select(Map).ToList();

            return queryResult;
        }
    }     
}

