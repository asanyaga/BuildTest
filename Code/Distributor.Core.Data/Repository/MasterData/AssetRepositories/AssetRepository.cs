using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.AssetRepositories;
using Distributr.Core.Repository.Master.CoolerTypeRepositories;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Resources.Util;
using Distributr.Core.Utility;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CoolerTypeRepositories
{
   internal class AssetRepository:RepositoryMasterBase<Asset>, IAssetRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        IAssetTypeRepository _assetTypeRepository;
        IAssetCategoryRepository _assetCategoryRepository;
        IAssetStatusRepository _assetStatusRepository;
      
       public AssetRepository(CokeDataContext ctx, ICacheProvider cacheProvider, IAssetTypeRepository assetTypeRepository, IAssetCategoryRepository assetCategoryRepository, IAssetStatusRepository assetStatusRepository)
       {
           _ctx = ctx;
           _cacheProvider = cacheProvider;
           _assetTypeRepository = assetTypeRepository;
           _assetCategoryRepository = assetCategoryRepository;
           _assetStatusRepository = assetStatusRepository;
         
       }

       public Guid Save(Asset entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            } 
           
            if (!vri.IsValid)
            {
                _log.Debug("Asset Not Valid");
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("asset.validation.error"));
            }
            DateTime dt = DateTime.Now;

            tblAsset casset = _ctx.tblAsset.FirstOrDefault(n => n.Id == entity.Id);
            if (casset == null )
            {
                casset = new tblAsset();
                casset.Id = entity.Id;
                casset.IM_Status = (int)EntityStatus.Active; //true;
                casset.IM_DateCreated = dt;
                casset.Id = entity.Id;
                _ctx.tblAsset.AddObject(casset);

            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (casset.IM_Status != (int)entityStatus)
                casset.IM_Status = (int)entity._Status;
            //else 
            //    cooler=  _ctx.tblCooler.FirstOrDefault(n => n.Id == entity.Id);
            casset.AssetTypeId = entity.AssetType.Id ;
            casset.AssetCategoryId = entity.AssetCategory.Id;
            casset.AssetStatusId = entity.AssetStatus.Id;
            casset.Code = entity.Code;
            casset.Name = entity.Name;
            casset.Capacity = entity.Capacity;
            casset.AssetNo = entity.AssetNo;
            casset.SerialNo = entity.SerialNo;
            casset.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblAsset.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, casset.Id));
            return casset.Id; 
        }

        public void SetInactive(Asset entity)
        {
            tblAsset cooler = _ctx.tblAsset.FirstOrDefault(n => n.Id == entity.Id);
            if (cooler != null)
            {
                cooler.IM_Status = (int)EntityStatus.Inactive;
                cooler.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblAsset.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, cooler.Id));
            }
        }

       public void SetActive(Asset entity)
       {
           tblAsset cooler = _ctx.tblAsset.FirstOrDefault(n => n.Id == entity.Id);
           if (cooler != null)
           {
               cooler.IM_Status = (int)EntityStatus.Active;
               cooler.IM_DateLastUpdated = DateTime.Now;
               _ctx.SaveChanges();
               _cacheProvider.Put(_cacheListKey, _ctx.tblAsset.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
               _cacheProvider.Remove(string.Format(_cacheKey, cooler.Id));
           }
       }

       public void SetAsDeleted(Asset entity)
       {
           tblAsset cooler = _ctx.tblAsset.FirstOrDefault(n => n.Id == entity.Id);
           if (cooler != null)
           {
               cooler.IM_Status = (int)EntityStatus.Deleted;
               cooler.IM_DateLastUpdated = DateTime.Now;
               _ctx.SaveChanges();
               _cacheProvider.Put(_cacheListKey, _ctx.tblAsset.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
               _cacheProvider.Remove(string.Format(_cacheKey, cooler.Id));
           }
       }

       public Asset GetById(Guid id, bool includeDeactivated = false)
        {
            Asset entity = (Asset)_cacheProvider.Get(string.Format(_cacheKey, id));
            if (entity == null)
            {
                var tbl = _ctx.tblAsset.FirstOrDefault(s => s.Id == id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public ValidationResultInfo Validate(Asset itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            bool hasDuplicateName = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(p => p.Name  == itemToValidate.Name );
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult("Duplicate Name found"));
            bool hasDuplicateCode = GetAll(true)
               .Where(s => s.Id != itemToValidate.Id)
               .Any(p => p.Code == itemToValidate.Code);
            if (hasDuplicateCode)
                vri.Results.Add(new ValidationResult("Duplicate Code found"));
           
            bool hasDuplicateAssetNo = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(p => p.AssetNo == itemToValidate.AssetNo);
            if (hasDuplicateAssetNo)
                vri.Results.Add(new ValidationResult("Duplicate AssetNo found"));
            
            bool hasDuplicateSerialNo = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(p => p.SerialNo == itemToValidate.SerialNo);
            if (hasDuplicateSerialNo)
                vri.Results.Add(new ValidationResult("Duplicate SerialNo found"));

            return vri;
        }


       protected override string _cacheKey
       {
           get { return "Asset-{0}"; }
       }

       protected override string _cacheListKey
       {
           get { return "AssetList"; }
       }

       public override IEnumerable<Asset> GetAll(bool includeDeactivated = false)
        {
            IList<Asset> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<Asset>(ids.Count);
                foreach (Guid id in ids)
                {
                    Asset entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblAsset.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (Asset p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }
            entities.ToList();
            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

       public QueryResult<Asset> Query(QueryStandard query)
       {
           IQueryable<tblAsset> assetQuery;

           if (query.ShowInactive)
               assetQuery = _ctx.tblAsset.Where(s => s.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
           else
               assetQuery = _ctx.tblAsset.Where(s => s.IM_Status == (int)EntityStatus.Active).AsQueryable();

           var queryResult = new QueryResult<Asset>(); 
           
           if (!string.IsNullOrWhiteSpace(query.Name))
               assetQuery =
                   assetQuery.Where(
                       s =>
                           s.AssetNo.ToLower().Contains(query.Name) ||
                           s.tblAssetType.Name.ToLower().Contains(query.Name) || s.Name.ToLower().Contains(query.Name) || s.Code.ToLower().Contains(query.Name));


           assetQuery = assetQuery.OrderBy(s => s.Name).ThenBy(s => s.Code);
           queryResult.Count = assetQuery.Count();
           
           if (query.Skip.HasValue && query.Take.HasValue)
               assetQuery = assetQuery.Skip(query.Skip.Value).Take(query.Take.Value);

           var result = assetQuery.ToList();
           
           queryResult.Data = result.Select(Map).OfType<Asset>().ToList();

           return queryResult;
       }

       public Asset Map(tblAsset cooler)
        {
            Asset cool = new Asset(cooler.Id )
            {
                AssetType=_assetTypeRepository.GetById(cooler.tblAssetType.Id) ,
                AssetStatus=_assetStatusRepository.GetById(cooler.AssetStatusId.Value),
                AssetCategory = _assetCategoryRepository.GetById(cooler.AssetCategoryId.Value),
                Code=cooler.Code,
                Name = cooler.Name,
                Capacity=cooler.Capacity,
                AssetNo=cooler.AssetNo,
                SerialNo=cooler.SerialNo,
                
            };
            cool._SetDateCreated(cooler.IM_DateCreated);
            cool._SetDateLastUpdated(cooler.IM_DateLastUpdated );
            cool._SetStatus((EntityStatus)cooler.IM_Status);

            return cool;
        }
    }
}
