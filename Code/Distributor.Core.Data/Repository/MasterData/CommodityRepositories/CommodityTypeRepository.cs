using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CoolerTypeRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CommodityRepositories
{
    internal class CommodityTypeRepository: RepositoryMasterBase<CommodityType>, ICommodityTypeRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        public CommodityTypeRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
            _ctx = ctx;
        }
        protected override string _cacheKey
        {
            get { return "CommodityType-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "CommodityTypeList"; }
        }

        public Guid Save(CommodityType entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            } 
           
            if (!vri.IsValid)
            {
                _log.Debug("Cooler Type not valid");
                throw new DomainValidationException(vri, "Commodity Type Entity Not valid");
            }
            DateTime dt = DateTime.Now;

            tblCommodityType commodityType = _ctx.tblCommodityType.FirstOrDefault(n => n.Id == entity.Id);
            if (commodityType == null)
            {
                commodityType = new tblCommodityType();
                commodityType.IM_Status = (int)EntityStatus.Active; //true;
                commodityType.IM_DateCreated = dt;
                commodityType.Id = entity.Id;
                _ctx.tblCommodityType.AddObject(commodityType);

            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (commodityType.IM_Status != (int)entityStatus)
                commodityType.IM_Status = (int)entity._Status;
            commodityType.Name = entity.Name;
            commodityType.Code = entity.Code;
            commodityType.Description = entity.Description;

            commodityType.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblCommodityType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, commodityType.Id));
            return commodityType.Id;
        }

        public void SetInactive(CommodityType entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasCommodityTypeDependencies = _ctx.tblCommodity.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.CommodityTypeId == entity.Id);

            if (hasCommodityTypeDependencies)
            {
                throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }
            else
            {
                tblCommodityType commodityType = _ctx.tblCommodityType.FirstOrDefault(n => n.Id == entity.Id);
                if (commodityType != null)
                {
                    commodityType.IM_Status = (int)EntityStatus.Inactive;
                    commodityType.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblCommodityType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, commodityType.Id));

                }
            }
        }

        public void SetActive(CommodityType entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasCommodityTypeDependencies = _ctx.tblCommodity.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.CommodityTypeId == entity.Id);
            if (hasCommodityTypeDependencies)
            {
                throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }
            tblCommodityType commodityType = _ctx.tblCommodityType.FirstOrDefault(n => n.Id == entity.Id);
            if (commodityType != null)
            {
                commodityType.IM_Status = (int)EntityStatus.Active;
                commodityType.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCommodityType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, commodityType.Id));

            }
        }

        public void SetAsDeleted(CommodityType entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasCoolerTypeDependencies = _ctx.tblCommodity.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.CommodityTypeId == entity.Id);

            if (hasCoolerTypeDependencies)
            {
                throw new DomainValidationException(vri, "Cannot delete\r\nDependencies found");
            }
            else
            {
                tblCommodityType commodityType = _ctx.tblCommodityType.FirstOrDefault(n => n.Id == entity.Id);
                if (commodityType != null)
                {
                    commodityType.IM_Status = (int)EntityStatus.Deleted;
                    commodityType.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblCommodityType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, commodityType.Id));

                }
            }
        }

        public CommodityType GetById(Guid Id, bool includeDeactivated = false)
        {
            CommodityType entity = (CommodityType)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblCommodityType.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public override IEnumerable<CommodityType> GetAll(bool includeDeactivated = false)
        {
            IList<CommodityType> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<CommodityType>(ids.Count);
                foreach (Guid id in ids)
                {
                    CommodityType entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblCommodityType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList();
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (CommodityType p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

      
        public QueryResult<CommodityType> Query(QueryStandard query)
        {
            IQueryable<tblCommodityType> commodityTypeQuery;

            if (query.ShowInactive)
                commodityTypeQuery = _ctx.tblCommodityType.Where(p => p.IM_Status != (int) EntityStatus.Deleted).AsQueryable();
            else
                commodityTypeQuery = _ctx.tblCommodityType.Where(p => p.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<CommodityType>();
            if(!string.IsNullOrEmpty(query.Name))
            {
               commodityTypeQuery= commodityTypeQuery.Where(
                    p =>
                    p.Name.ToLower().Contains(query.Name.ToLower()) ||
                    p.Description.ToLower().Contains(query.Name.ToLower()) || p.Code.ToLower().Contains(query.Name));
            }

            commodityTypeQuery=commodityTypeQuery.OrderBy(p => p.Name).ThenBy(p => p.Code);
            queryResult.Count = commodityTypeQuery.Count();

            if(query.Skip.HasValue&& query.Take.HasValue)
                commodityTypeQuery = commodityTypeQuery.Skip(query.Skip.Value).Take(query.Take.Value);

            queryResult.Data = commodityTypeQuery.Select(Map).OfType<CommodityType>().ToList();

            return queryResult;
        }

        #region Implementation of IValidation<CommodityType>

        public ValidationResultInfo Validate(CommodityType itemToValidate)
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
        public CommodityType Map(tblCommodityType commodityType)
        {
            CommodityType commodity = new CommodityType(commodityType.Id)
            {
                Name = commodityType.Name,
                Code = commodityType.Code,
                Description = commodityType.Description

            };
            commodity._SetDateCreated(commodityType.IM_DateCreated);
            commodity._SetDateLastUpdated(commodityType.IM_DateLastUpdated);
            commodity._SetStatus((EntityStatus)commodityType.IM_Status);
            return commodity;
        }

        #endregion
    }
}
