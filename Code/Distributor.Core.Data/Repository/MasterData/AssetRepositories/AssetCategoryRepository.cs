using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.AssetEntities;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.AssetRepositories;
using Distributr.Core.Repository.Master.CoolerTypeRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.AssetRepositories
{
    internal class AssetCategoryRepository:RepositoryMasterBase<AssetCategory>, IAssetCategoryRepository
    {
        private IAssetTypeRepository _assetTypeRepository;
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;

        public AssetCategoryRepository(IAssetTypeRepository assetTypeRepository, CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _assetTypeRepository = assetTypeRepository;
            _ctx = ctx;
            _cacheProvider = cacheProvider;
        }

        public Guid Save(AssetCategory entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }

            if (!vri.IsValid)
            {
                string info = string.Join(",", vri.Results.Select(n => n.ErrorMessage));
                _log.Debug("Asset Category  not valid");
                throw new DomainValidationException(vri, "Asset Category Entity Not valid  " +   info);
            }
            DateTime dt = DateTime.Now;

            tblAssetCategory assetCategory = _ctx.tblAssetCategory.FirstOrDefault(n => n.Id == entity.Id);
            if (assetCategory == null)
            {
                assetCategory = new tblAssetCategory();
                assetCategory.IM_Status = (int)EntityStatus.Active;
                assetCategory.IM_DateCreated = dt;
                assetCategory.Id = entity.Id;
               
                _ctx.tblAssetCategory.AddObject(assetCategory);

            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (assetCategory.IM_Status != (int)entityStatus)
                assetCategory.IM_Status = (int)entity._Status;
              
            assetCategory.Name = entity.Name;
            assetCategory.Description = entity.Description;
            assetCategory.AssetTypeId = entity.AssetType.Id;
            assetCategory.IM_DateLastUpdated = dt;

            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblAssetCategory.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, assetCategory.Id));
            return assetCategory.Id; 
        }

        public void SetInactive(AssetCategory entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasCoolerTypeDependencies = _ctx.tblAsset.Where(s => s.IM_Status ==(int)EntityStatus.Active).Any(p => p.AssetCategoryId == entity.Id);
            if (hasCoolerTypeDependencies)
            {
                throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }
            else
            {
                tblAssetCategory assetcategory = _ctx.tblAssetCategory.FirstOrDefault(n => n.Id == entity.Id);
                if (assetcategory != null)
                {
                    assetcategory.IM_Status = (int)EntityStatus.Inactive; //false;
                    assetcategory.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblAssetCategory.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, assetcategory.Id));
                }
            }
        }

        public void SetActive(AssetCategory entity)
        {
            tblAssetCategory tblAssetCategory = _ctx.tblAssetCategory.FirstOrDefault(n => n.Id == entity.Id);
            if(tblAssetCategory != null)
            {
                tblAssetCategory.IM_Status = (int) EntityStatus.Active;
                tblAssetCategory.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey,_ctx.tblAssetCategory.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(n => n.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey,tblAssetCategory.Id));
            }
        }

        public void SetAsDeleted(AssetCategory entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasCoolerTypeDependencies = _ctx.tblAsset.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.AssetCategoryId == entity.Id);
            if (hasCoolerTypeDependencies)
            {
                throw new DomainValidationException(vri, "Cannot Delete\r\nDependencies found");
            }
            else
            {
                tblAssetCategory assetcategory = _ctx.tblAssetCategory.FirstOrDefault(n => n.Id == entity.Id);
                if (assetcategory != null)
                {
                    assetcategory.IM_Status = (int)EntityStatus.Deleted; //false;
                    assetcategory.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblAssetCategory.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, assetcategory.Id));
                }
            }
        }

        public AssetCategory GetById(Guid Id, bool includeDeactivated=false)
        {
            AssetCategory entity = (AssetCategory)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblAssetCategory.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        protected override string _cacheKey
        {
            get { return "AssetCategory-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "AssetCategoryList"; }
        }

        public override IEnumerable<AssetCategory> GetAll(bool includeDeactivated=false)
        {
            IList<AssetCategory> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<AssetCategory>(ids.Count);
                foreach (Guid id in ids)
                {
                    AssetCategory entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblAssetCategory.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (AssetCategory p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public QueryResult<AssetCategory> Query(QueryStandard q)
        {
            IQueryable<tblAssetCategory> assetCartegoryResult;

            if (q.ShowInactive)
                assetCartegoryResult =
                    _ctx.tblAssetCategory.Where(n => n.IM_Status != (int) EntityStatus.Deleted).AsQueryable();
            else
                assetCartegoryResult = _ctx.tblAssetCategory.Where(n => n.IM_Status == (int) EntityStatus.Active);

            var queryResult = new QueryResult<AssetCategory>();
            
            if (!string.IsNullOrWhiteSpace(q.Name))
                assetCartegoryResult = assetCartegoryResult.Where(s => s.Name.ToLower().Contains(q.Name) || s.Description.ToLower().Contains(q.Name));

            assetCartegoryResult = assetCartegoryResult.OrderBy(s => s.Name).ThenBy(s => s.Description);

            queryResult.Count = assetCartegoryResult.Count();
            if (q.Take.HasValue && q.Skip.HasValue)
                assetCartegoryResult = assetCartegoryResult.Take(q.Take.Value).Skip(q.Skip.Value);

            queryResult.Data = assetCartegoryResult.Select(Map).OfType<AssetCategory>().ToList();

            q.ShowInactive = false;

            return queryResult;
        }

        public AssetCategory Map(tblAssetCategory assetCategory)
        {
            AssetCategory assetCate = new AssetCategory(assetCategory.Id)
            {
                Name = assetCategory.Name,
                Description = assetCategory.Description
            };
            assetCate.AssetType = _assetTypeRepository.GetById(assetCategory.AssetTypeId);
            assetCate._SetDateCreated(assetCategory.IM_DateCreated);
            assetCate._SetDateLastUpdated(assetCategory.IM_DateLastUpdated);
            assetCate._SetStatus((EntityStatus)assetCategory.IM_Status);
            return assetCate;
        }
        public ValidationResultInfo Validate(AssetCategory itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            var duplicates = GetAll(true).Where(s => s.Id != itemToValidate.Id);
            bool hasDuplicateName = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id && s.AssetType.Id == itemToValidate.AssetType.Id)
                .Any(s => s.Name == itemToValidate.Name);
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult("Duplicate Name found"));
            return vri;
        }
    }
}
    