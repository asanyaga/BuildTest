using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.CoolerTypeRepositories;
using Distributr.Core.Domain.Master.CoolerEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.IOC;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Utility;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CoolerTypeRepositories
{
    internal  class AssetTypeRepository : RepositoryMasterBase<AssetType>, IAssetTypeRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        public AssetTypeRepository(CokeDataContext ctx,ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
            _ctx = ctx;
        }
        public Guid Save(AssetType entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            } 
           
            if (!vri.IsValid)
            {
                _log.Debug("Cooler Type not valid");
                throw new DomainValidationException(vri, "Cooler Type Entity Not valid");
            }
            DateTime dt = DateTime.Now;

            tblAssetType coolerType = _ctx.tblAssetType.FirstOrDefault(n => n.Id == entity.Id);
            if (coolerType == null)
            {
                coolerType = new tblAssetType();
                coolerType.IM_Status = (int)EntityStatus.Active; //true;
                coolerType.IM_DateCreated = dt;
                coolerType.Id = entity.Id;
                _ctx.tblAssetType.AddObject(coolerType);

            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (coolerType.IM_Status != (int)entityStatus)
                coolerType.IM_Status = (int)entity._Status;
            coolerType.Name = entity.Name;
            coolerType.Code = entity.Description ;
           
            coolerType.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblAssetType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, coolerType.Id));
            return coolerType.Id; 
        }

        public void SetInactive(AssetType entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasCoolerTypeDependencies = _ctx.tblAsset.Where(s => s.IM_Status ==(int)EntityStatus.Active).Any(p => p.AssetTypeId == entity.Id);

            if (hasCoolerTypeDependencies)
            {
                throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }
            else
            {
                tblAssetType coolerType = _ctx.tblAssetType.FirstOrDefault(n => n.Id == entity.Id);
                if (coolerType != null)
                {
                    coolerType.IM_Status = (int)EntityStatus.Inactive;
                    coolerType.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblAssetType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, coolerType.Id));
                    
                }
            }
        }

        public void SetActive(AssetType entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasCoolerTypeDependencies = _ctx.tblAsset.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.AssetTypeId == entity.Id);
                        
                tblAssetType coolerType = _ctx.tblAssetType.FirstOrDefault(n => n.Id == entity.Id);
                if (coolerType != null)
                {
                    coolerType.IM_Status = (int)EntityStatus.Active;
                    coolerType.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblAssetType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, coolerType.Id));

                }
            
        }

        public void SetAsDeleted(AssetType entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasCoolerTypeDependencies = _ctx.tblAsset
                .Where(s => s.IM_Status != (int)EntityStatus.Deleted)
                .Any(p => p.AssetTypeId == entity.Id);
            if (!hasCoolerTypeDependencies)
                hasCoolerTypeDependencies = _ctx.tblAssetCategory
                    .Where(s => s.IM_Status != (int)EntityStatus.Deleted)
                    .Any(n => n.AssetTypeId == entity.Id);
            if (hasCoolerTypeDependencies)
            {
                throw new DomainValidationException(vri, "Cannot delete\r\nDependencies found");
            }
            else
            {
                tblAssetType coolerType = _ctx.tblAssetType.FirstOrDefault(n => n.Id == entity.Id);
                if (coolerType != null)
                {
                    coolerType.IM_Status = (int)EntityStatus.Deleted;
                    coolerType.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblAssetType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, coolerType.Id));

                }
            }
        }

        public AssetType GetById(Guid id, bool includeDeactivated = false)
        {
            AssetType entity = (AssetType)_cacheProvider.Get(string.Format(_cacheKey, id));
            if (entity == null)
            {
                var tbl = _ctx.tblAssetType.FirstOrDefault(s => s.Id == id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public ValidationResultInfo Validate(AssetType itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            bool hasDuplicateName = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(p => p.Name == itemToValidate.Name);
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult("Duplicate Name found"));
            return vri;
        }


        protected override string _cacheKey
        {
            get { return "AssetType-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "AssetTypeList"; }
        }

        public override IEnumerable<AssetType> GetAll(bool includeDeactivated = false)
        {
            IList<AssetType> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<AssetType>(ids.Count);
                foreach (Guid id in ids)
                {
                    AssetType entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblAssetType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); 
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (AssetType p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public QueryResult<AssetType> Query(QueryStandard query)
        {
            IQueryable<tblAssetType> assetTypeQuery;
            if (query.ShowInactive)
                assetTypeQuery = _ctx.tblAssetType.Where(p => p.IM_Status != (int) EntityStatus.Deleted).AsQueryable();
            else
                assetTypeQuery = _ctx.tblAssetType.Where(p => p.IM_Status == (int)EntityStatus.Active).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
                assetTypeQuery =
                    assetTypeQuery.Where(
                        q =>
                            q.Name.ToLower().Contains(query.Name.ToLower()) ||
                            q.Code.ToLower().Contains(query.Name.ToLower()));
            
            var queryResult = new QueryResult<AssetType>();
            queryResult.Count = assetTypeQuery.Count();

            assetTypeQuery = assetTypeQuery.OrderBy(m => m.Name).ThenBy(m => m.Code);

            if (query.Skip.HasValue && query.Take.HasValue)
                assetTypeQuery = assetTypeQuery.Skip(query.Skip.Value).Take(query.Take.Value);

            var result = assetTypeQuery.ToList();

            queryResult.Data = result.Select(Map).ToList();

            query.ShowInactive = false;

            return queryResult;
        }


        public AssetType Map(tblAssetType coolerType)
        {
            AssetType cooler = new AssetType(coolerType.Id)
            {
                Name=coolerType.Name,
                Description=coolerType.Code 
            };
            cooler._SetDateCreated(coolerType.IM_DateCreated );
            cooler._SetDateLastUpdated(coolerType.IM_DateLastUpdated );
            cooler._SetStatus((EntityStatus)coolerType.IM_Status );
            return cooler;
        }
    }
}
