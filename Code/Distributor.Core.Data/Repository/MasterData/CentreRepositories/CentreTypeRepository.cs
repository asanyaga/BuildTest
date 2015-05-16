using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CentreEntity;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.CentreRepositories;
using Distributr.Core.Repository.Master.MasterDataAllocationRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CentreRepositories
{
    class CentreTypeRepository : RepositoryMasterBase<CentreType>, ICentreTypeRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;

        public CentreTypeRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
        }

        protected override string _cacheKey
        {
            get { return "CentreType-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "CentreTypeList"; }
        }

        public Guid Save(CentreType entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                _log.Debug(CoreResourceHelper.GetText("centretype.validation.error"));
                throw new DomainValidationException(vri, ""/*CoreResourceHelper.GetText("centretype.validation.error")*/);
            }
            DateTime dt = DateTime.Now;

            tblCentreType centretype = _ctx.tblCentreType.FirstOrDefault(n => n.Id == entity.Id);
            if (centretype == null)
            {
                centretype = new tblCentreType();
                centretype.Id = entity.Id;
                centretype.IM_Status = (int)EntityStatus.Active;
                centretype.IM_DateCreated = dt;
                _ctx.tblCentreType.AddObject(centretype);

            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (centretype.IM_Status != (int)entityStatus)
                centretype.IM_Status = (int)entity._Status;
            centretype.Code = entity.Code;
            centretype.Description = entity.Description;
            centretype.Name = entity.Name;
            centretype.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblCentreType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, centretype.Id));
            return centretype.Id; 
        }

        public void SetInactive(CentreType entity)
        {
            tblCentreType centretype = _ctx.tblCentreType.FirstOrDefault(n => n.Id == entity.Id);
            if (centretype != null)
            {
                centretype.IM_Status = (int)EntityStatus.Inactive;
                centretype.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCentreType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, centretype.Id));
            }
        }

        public void SetActive(CentreType entity)
        {
            tblCentreType centretype = _ctx.tblCentreType.FirstOrDefault(n => n.Id == entity.Id);
            if (centretype != null)
            {
                centretype.IM_Status = (int)EntityStatus.Active;
                centretype.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCentreType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, centretype.Id));
            }
        }

        public void SetAsDeleted(CentreType entity)
        {
            tblCentreType centretype = _ctx.tblCentreType.FirstOrDefault(n => n.Id == entity.Id);
            if (centretype != null)
            {
                centretype.IM_Status = (int)EntityStatus.Deleted;
                centretype.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblCentreType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, centretype.Id));
            }
        }

        public CentreType GetById(Guid id, bool includeDeactivated = false)
        {
            CentreType entity = (CentreType)_cacheProvider.Get(string.Format(_cacheKey, id));
            if (entity == null)
            {
                var tbl = _ctx.tblCentreType.FirstOrDefault(s => s.Id == id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public override IEnumerable<CentreType> GetAll(bool includeDeactivated = false)
        {
            IList<CentreType> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<CentreType>(ids.Count);
                foreach (Guid id in ids)
                {
                    CentreType entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblCentreType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList();
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (CentreType p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }
          
            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public QueryResult<CentreType> Query(QueryStandard q)
        {
            IQueryable<tblCentreType> centerQuery;
            if (q.ShowInactive)
                centerQuery = _ctx.tblCentreType.Where(s => s.IM_Status == (int)EntityStatus.Active || s.IM_Status==(int)EntityStatus.Inactive).AsQueryable();
            else
                centerQuery = _ctx.tblCentreType.Where(s => s.IM_Status == (int)EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<CentreType>();
            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                centerQuery = centerQuery
                    .Where(s => s.Name.ToLower().Contains(q.Name.ToLower()));
            }

            queryResult.Count = centerQuery.Count();
            centerQuery = centerQuery.OrderBy(s => s.Name).ThenBy(s => s.Code);
            if (q.Skip.HasValue && q.Take.HasValue)
                centerQuery = centerQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = centerQuery.ToList();
            queryResult.Data = result.Select(Map).OfType<CentreType>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }

        public ValidationResultInfo Validate(CentreType itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            bool hasDuplicateName = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(p => p.Name.ToLower() == itemToValidate.Name.ToLower());
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult("Duplicate Name found"));
            bool hasDuplicateCode = GetAll(true)
               .Where(s => s.Id != itemToValidate.Id)
               .Any(p => p.Code.ToLower() == itemToValidate.Code.ToLower());
            if (hasDuplicateCode)
                vri.Results.Add(new ValidationResult("Duplicate Code found"));

            return vri;
        }

        public CentreType Map(tblCentreType centretbl)
        {
            CentreType centre = new CentreType(centretbl.Id)
            {
                Code = centretbl.Code,
                Description = centretbl.Description,
                Name = centretbl.Name
            };

            centre._SetDateCreated(centretbl.IM_DateCreated);
            centre._SetDateLastUpdated(centretbl.IM_DateLastUpdated);
            centre._SetStatus((EntityStatus)centretbl.IM_Status);

            return centre;
        }
    }
}
