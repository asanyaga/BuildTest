using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class CommodityOwnerTypeRepository : RepositoryMasterBase<CommodityOwnerType>, ICommodityOwnerTypeRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;

        public CommodityOwnerTypeRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
        }

        public Guid Save(CommodityOwnerType entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            } 
           
            if (!vri.IsValid)
            {
                _log.Debug("Commodity Owner Type not valid");
                throw new DomainValidationException(vri, "Commodity Owner Type Entity Not valid");
            }

            DateTime dt = DateTime.Now;

            tblCommodityOwnerType commodityOwnerType = _ctx.tblCommodityOwnerType.FirstOrDefault(n => n.Id == entity.Id);
            if (commodityOwnerType == null)
            {
                commodityOwnerType = new tblCommodityOwnerType();
                commodityOwnerType.IM_Status = (int)EntityStatus.Active;
                commodityOwnerType.IM_DateCreated = dt;
                commodityOwnerType.Id = entity.Id;
                _ctx.tblCommodityOwnerType.AddObject(commodityOwnerType);

            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (commodityOwnerType.IM_Status != (int)entityStatus)
                commodityOwnerType.IM_Status = (int)entity._Status;
            commodityOwnerType.Name = entity.Name;
            commodityOwnerType.Code = entity.Code;
            commodityOwnerType.Description = entity.Description;

            commodityOwnerType.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblCommodityOwnerType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, commodityOwnerType.Id));
            return commodityOwnerType.Id; 
        }

        public void SetInactive(CommodityOwnerType entity)
        {
             ValidationResultInfo vri = Validate(entity);
             bool hasCommodityOwnerDependencies = _ctx.tblCommodityOwner.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.CommodityOwnerTypeId == entity.Id);

             if (hasCommodityOwnerDependencies)
            {
                throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }
            else
            {
                tblCommodityOwnerType commodityOwnerType = _ctx.tblCommodityOwnerType.FirstOrDefault(n => n.Id == entity.Id);
                if (commodityOwnerType != null)
                {
                    commodityOwnerType.IM_Status = (int)EntityStatus.Inactive;
                    commodityOwnerType.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblCommodityOwnerType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, commodityOwnerType.Id));
                    
                }
            }
        }

        public void SetActive(CommodityOwnerType entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasCommodityOwnerDependencies = _ctx.tblCommodityOwner.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.CommodityOwnerTypeId == entity.Id);

            if (hasCommodityOwnerDependencies)
            {
                throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }
            else
            {
                tblCommodityOwnerType commodityOwnerType = _ctx.tblCommodityOwnerType.FirstOrDefault(n => n.Id == entity.Id);
                if (commodityOwnerType != null)
                {
                    commodityOwnerType.IM_Status = (int)EntityStatus.Active;
                    commodityOwnerType.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblCommodityOwnerType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, commodityOwnerType.Id));

                }
            }
        }

        public void SetAsDeleted(CommodityOwnerType entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasCommodityOwnerDependencies = _ctx.tblCommodityOwner.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.CommodityOwnerTypeId == entity.Id);

            if (hasCommodityOwnerDependencies)
            {
                throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }
            else
            {
                tblCommodityOwnerType commodityOwnerType = _ctx.tblCommodityOwnerType.FirstOrDefault(n => n.Id == entity.Id);
                if (commodityOwnerType != null)
                {
                    commodityOwnerType.IM_Status = (int)EntityStatus.Deleted;
                    commodityOwnerType.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblCommodityOwnerType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, commodityOwnerType.Id));

                }
            }
        }

        public CommodityOwnerType GetById(Guid Id, bool includeDeactivated = false)
        {
            CommodityOwnerType entity = (CommodityOwnerType)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblCommodityOwnerType.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public ValidationResultInfo Validate(CommodityOwnerType itemToValidate)
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
            get { return "CommodityOwnerType-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "CommodityOwnerTypeList"; }
        }

        public override IEnumerable<CommodityOwnerType> GetAll(bool includeDeactivated = false)
        {
            IList<CommodityOwnerType> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<CommodityOwnerType>(ids.Count);
                foreach (Guid id in ids)
                {
                    CommodityOwnerType entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblCommodityOwnerType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList();
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (CommodityOwnerType p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public CommodityOwnerType Map(tblCommodityOwnerType commodityOwnerType)
        {
            CommodityOwnerType commodityOwner = new CommodityOwnerType(commodityOwnerType.Id)
            {
                Name = commodityOwnerType.Name,
                Description = commodityOwnerType.Description,
                Code = commodityOwnerType.Code
            };
            commodityOwner._SetDateCreated(commodityOwnerType.IM_DateCreated);
            commodityOwner._SetDateLastUpdated(commodityOwnerType.IM_DateLastUpdated);
            commodityOwner._SetStatus((EntityStatus)commodityOwnerType.IM_Status);
            return commodityOwner;
        }

        public QueryResult<CommodityOwnerType> Query(QueryStandard query)
        {
            IQueryable<tblCommodityOwnerType> commodityOwnerQuery;

            if (query.ShowInactive)
                commodityOwnerQuery =
                    _ctx.tblCommodityOwnerType.Where(p => p.IM_Status != (int) EntityStatus.Deleted).AsQueryable();
            else
            {
                commodityOwnerQuery =
                    _ctx.tblCommodityOwnerType.Where(p => p.IM_Status == (int) EntityStatus.Active).AsQueryable();
            }

            var queryResult = new QueryResult<CommodityOwnerType>();

            if (!string.IsNullOrEmpty(query.Name))
            {
                commodityOwnerQuery =
                    commodityOwnerQuery.Where(
                        p =>
                            p.Code.ToLower().Contains(query.Name.ToLower()) ||
                            p.Name.ToLower().Contains(query.Name.ToLower()));
            }

            commodityOwnerQuery = commodityOwnerQuery.OrderBy(p => p.Code).ThenBy(p => p.Name);

            queryResult.Count = commodityOwnerQuery.Count();

            if (query.Skip.HasValue && query.Take.HasValue)
            {
                commodityOwnerQuery = commodityOwnerQuery.Skip(query.Skip.Value).Take(query.Take.Value);
            }

            queryResult.Data = commodityOwnerQuery.Select(Map).OfType<CommodityOwnerType>().ToList();
            return queryResult;
        }
    
    }

}
