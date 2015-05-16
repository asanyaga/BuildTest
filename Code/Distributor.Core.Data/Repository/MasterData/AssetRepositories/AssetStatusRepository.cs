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
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.AssetRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.AssetRepositories
{
    internal class AssetStatusRepository:RepositoryMasterBase<AssetStatus>, IAssetStatusRepository
    {

        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;

        public AssetStatusRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
        }



        public Guid Save(AssetStatus entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            } 
           
            if (!vri.IsValid)
            {
                _log.Debug("Asset Status  not valid");
                throw new DomainValidationException(vri, "Asset Status Entity Not valid");
            }
            DateTime dt = DateTime.Now;

            tblAssetStatus assetstatus = _ctx.tblAssetStatus.FirstOrDefault(n => n.Id == entity.Id);
            if (assetstatus == null)
            {
                assetstatus = new tblAssetStatus();
                assetstatus.Id = entity.Id;
                assetstatus.IM_Status = (int)EntityStatus.Active; //true;
                assetstatus.IM_DateCreated = dt;
                _ctx.tblAssetStatus.AddObject(assetstatus);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (assetstatus.IM_Status != (int)entityStatus)
                assetstatus.IM_Status = (int)entity._Status;
            assetstatus.Name = entity.Name;
            assetstatus.Description = entity.Description;

            assetstatus.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblAssetStatus.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, assetstatus.Id));
            return assetstatus.Id; 
        }

        public void SetInactive(AssetStatus entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasCoolerTypeDependencies = _ctx.tblAsset.Where(s => s.IM_Status ==(int)EntityStatus.Active).Any(p => p.AssetStatusId == entity.Id);

            if (hasCoolerTypeDependencies)
            {
                throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }
            else
            {
                tblAssetStatus assetStatus = _ctx.tblAssetStatus.FirstOrDefault(n => n.Id == entity.Id);
                if (assetStatus != null)
                {
                    assetStatus.IM_Status = (int)EntityStatus.Inactive;
                    assetStatus.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblAssetStatus.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, assetStatus.Id));

                }
            } 
        }

        public void SetActive(AssetStatus entity)
        {
            tblAssetStatus assetStatus = _ctx.tblAssetStatus.FirstOrDefault(n => n.Id == entity.Id);
            if(assetStatus!=null)
            {
                assetStatus.IM_Status = (int) EntityStatus.Active;
                assetStatus.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey,_ctx.tblAssetStatus.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(n => n.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, assetStatus.Id));
            }
        }

        public void SetAsDeleted(AssetStatus entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasCoolerTypeDependencies = _ctx.tblAsset
                .Where(s => s.IM_Status != (int)EntityStatus.Deleted).Any(p => p.AssetStatusId == entity.Id);
            if (hasCoolerTypeDependencies)
            {
                throw new DomainValidationException(vri, "Cannot Delete\r\nDependencies found");
            }
            else
            {
                tblAssetStatus assetStatus = _ctx.tblAssetStatus.FirstOrDefault(n => n.Id == entity.Id);
                if (assetStatus != null)
                {
                    assetStatus.IM_Status = (int)EntityStatus.Deleted;
                    assetStatus.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblAssetStatus.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, assetStatus.Id));

                }
            } 
        }

        public AssetStatus GetById(Guid Id, bool includeDeactivated=false)
        {
            AssetStatus entity = (AssetStatus)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblAssetStatus.FirstOrDefault(s => s.Id == Id);
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
            get { return "AssetStatus-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "AssetStatusList"; }
        }

        public override IEnumerable<AssetStatus> GetAll(bool includeDeactivated=false)
        {
            IList<AssetStatus> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<AssetStatus>(ids.Count);
                foreach (Guid id in ids)
                {
                    AssetStatus entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblAssetStatus.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (AssetStatus p in entities)
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

        public QueryResult<AssetStatus> Query(QueryStandard q)
        {
            IQueryable<tblAssetStatus> assetStatusQuery;

            if (q.ShowInactive)
                assetStatusQuery =
                    _ctx.tblAssetStatus.Where(s => s.IM_Status != (int) EntityStatus.Deleted).AsQueryable();
            else
                assetStatusQuery =
                    _ctx.tblAssetStatus.Where(s => s.IM_Status == (int) EntityStatus.Active).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q.Name))
                assetStatusQuery =
                    assetStatusQuery.Where(
                        s => s.Name.ToLower().Contains(q.Name) || s.Description.ToLower().Contains(q.Name));
            
            var queryResult = new QueryResult<AssetStatus>();
            queryResult.Count = assetStatusQuery.Count();

            assetStatusQuery = assetStatusQuery.OrderBy(s => s.Name).ThenBy(s => s.Description);

            if (q.Skip.HasValue && q.Take.HasValue)
                assetStatusQuery = assetStatusQuery.Skip(q.Skip.Value).Take(q.Take.Value);

            var result = assetStatusQuery.ToList();

            queryResult.Data = result.Select(Map).ToList();

            q.ShowInactive = false;

            return queryResult;
        }

        public AssetStatus Map(tblAssetStatus assetStatus)
        {
            AssetStatus assetstatus = new AssetStatus(assetStatus.Id)
            {
                Name = assetStatus.Name,
                Description = assetStatus.Description
            };
            assetstatus._SetDateCreated(assetStatus.IM_DateCreated);
            assetstatus._SetDateLastUpdated(assetStatus.IM_DateLastUpdated);
            assetstatus._SetStatus((EntityStatus)assetStatus.IM_Status);
            return assetstatus;
        }
        public ValidationResultInfo Validate(AssetStatus itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            bool hasDuplicateName = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(p => p.Name.ToString() == itemToValidate.Name);
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult("Duplicate Name found"));
            return vri;
        }
    }
}
