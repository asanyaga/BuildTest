using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility.Caching;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.ProductRepositories
{
   internal class DiscountGroupRepository:RepositoryMasterBase<DiscountGroup>,IDiscountGroupRepository
    {
       CokeDataContext _ctx;
       ICacheProvider _cacheProvider;

       public DiscountGroupRepository(CokeDataContext ctx,ICacheProvider cacheProvider)
       {
           _ctx = ctx;
           _cacheProvider = cacheProvider;
       }

       public Guid Save(DiscountGroup entity, bool? isSync = null)
        {
            _log.InfoFormat("Saving/Updating Group discount");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                throw new DomainValidationException(vri,"Failed to validate group discount");
            }
            DateTime dt = DateTime.Now;
            tblDiscountGroup tblDG = _ctx.tblDiscountGroup.FirstOrDefault(n => n.id == entity.Id);
            if (tblDG == null)
            {
                tblDG = new tblDiscountGroup();
                tblDG.IM_DateCreated = dt;
                tblDG.IM_Status = (int)EntityStatus.Active;// true;
                tblDG.id = entity.Id;
                _ctx.tblDiscountGroup.AddObject(tblDG);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tblDG.IM_Status != (int)entityStatus)
                tblDG.IM_Status = (int)entity._Status;
              
            tblDG.IM_DateLastUpdated = dt;
            tblDG.Name = entity.Name;
            tblDG.Code = entity.Code;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblDiscountGroup.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tblDG.id));
            return tblDG.id;
        }

        public void SetInactive(DiscountGroup entity)
        {
            ValidationResultInfo vri = Validate(entity);
           bool hasCostCentreDependencies = 
               _ctx.tblCostCentre.Where(s => s.IM_Status == (int)EntityStatus.Active)
               .Any(p => p.Outlet_DiscountGroupId == entity.Id);
           if (hasCostCentreDependencies)
           {
               throw new DomainValidationException(vri, "Cannot Delete Discount Group Cost-Centre dependency found");
           }
           else
           {
               _log.InfoFormat("Deactivating group discount");
               tblDiscountGroup tblDG = _ctx.tblDiscountGroup.FirstOrDefault(n => n.id == entity.Id);
               if (tblDG != null)
               {
                   tblDG.IM_DateLastUpdated = DateTime.Now;
                   tblDG.IM_Status = (int) EntityStatus.Inactive; // false;
                   _ctx.SaveChanges();
                   _cacheProvider.Put(_cacheListKey,
                                      _ctx.tblDiscountGroup.Where(n => n.IM_Status != (int) EntityStatus.Deleted).Select
                                          (s => s.id).ToList());
                   _cacheProvider.Remove(string.Format(_cacheKey, tblDG.id));
               }
           }

        }

       public void SetActive(DiscountGroup entity)
       {
           _log.InfoFormat("Activating Group discount");
           ValidationResultInfo vri = Validate(entity);
           if (!vri.IsValid)
           {
               throw new DomainValidationException(vri, "Failed to validate group discount");
           }
           tblDiscountGroup tblDG = _ctx.tblDiscountGroup.FirstOrDefault(n => n.id == entity.Id);
           if (tblDG != null)
           {
               tblDG.IM_DateLastUpdated = DateTime.Now;
               tblDG.IM_Status = (int)EntityStatus.Active;// false;
               _ctx.SaveChanges();
               _cacheProvider.Put(_cacheListKey, _ctx.tblDiscountGroup.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
               _cacheProvider.Remove(string.Format(_cacheKey, tblDG.id));
           }
       }

       public void SetAsDeleted(DiscountGroup entity)
       {
           ValidationResultInfo vri = Validate(entity);
           bool hasCostCentreDependencies = _ctx.tblCostCentre.Where(s => s.IM_Status != (int)EntityStatus.Deleted)
               .Any(p => p.Outlet_DiscountGroupId == entity.Id);
           if (hasCostCentreDependencies)
           {
               throw new DomainValidationException(vri, "Cannot Delete Discount Group Cost-Centre dependency found");
           }
           else
           {
               _log.InfoFormat("Deleting discount group");
               tblDiscountGroup tblDG = _ctx.tblDiscountGroup.FirstOrDefault(n => n.id == entity.Id);
               if (tblDG != null)
               {
                   tblDG.IM_DateLastUpdated = DateTime.Now;
                   tblDG.IM_Status = (int)EntityStatus.Deleted; // false;
                   _ctx.SaveChanges();
                   _cacheProvider.Put(_cacheListKey,
                                      _ctx.tblDiscountGroup.Where(n => n.IM_Status != (int)EntityStatus.Deleted).
                                          Select(s => s.id).ToList());
                   _cacheProvider.Remove(string.Format(_cacheKey, tblDG.id));
               }
           }
       }

       public DiscountGroup GetById(Guid Id, bool includeDeactivated = false)
        {
            DiscountGroup entity = (DiscountGroup)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblDiscountGroup.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public ValidationResultInfo Validate(DiscountGroup itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
           bool hasDuplicateCode = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(p=>p.Code==itemToValidate.Code);
            if(hasDuplicateCode)
                vri.Results.Add(new ValidationResult("Duplicate Code found"));
            bool hasDuplicateName = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(p => p.Name == itemToValidate.Name);
            if(hasDuplicateName)
                 vri.Results.Add(new ValidationResult("Duplicate Name found"));
            return vri;
        }

       protected override string _cacheKey
       {
           get { return "DiscountGroup-{0}"; }
       }

       protected override string _cacheListKey
       {
           get { return "DiscountGroupList"; }
       }

       public override IEnumerable<DiscountGroup> GetAll(bool includeDeactivated = false)
        {
            _log.InfoFormat("Get all discount groups");
            IList<DiscountGroup> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<DiscountGroup>(ids.Count);
                foreach (Guid id in ids)
                {
                    DiscountGroup entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblDiscountGroup.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (DiscountGroup p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

       public DiscountGroup GetByCode(string code)
       {
           var item = _ctx.tblDiscountGroup.FirstOrDefault(p => p.Code != null && p.Code.ToLower() == code.ToLower());
           if (item != null)
               return Map(item);
           return null;
       }

       public QueryResult<DiscountGroup> QueryResult(QueryStandard q)
       {
           IQueryable<tblDiscountGroup> dgQuery;

           if (q.ShowInactive)
               dgQuery = _ctx.tblDiscountGroup.Where(j => j.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
           else
               dgQuery = _ctx.tblDiscountGroup.Where(h => h.IM_Status == (int) EntityStatus.Active).AsQueryable();

           if (!string.IsNullOrEmpty(q.Name))
               dgQuery = dgQuery.Where(h => h.Code.ToLower().Contains(q.Name) || h.Name.ToLower().Contains(q.Name));

   
           var queryResult = new QueryResult<DiscountGroup>();

           queryResult.Count = dgQuery.Count();

           dgQuery = dgQuery.OrderBy(g => g.Name).ThenBy(h => h.Code);

           if (q.Skip.HasValue && q.Take.HasValue)
               dgQuery = dgQuery.Skip(q.Skip.Value).Take(q.Take.Value);

           var results = dgQuery.ToList();

           queryResult.Data = results.Select(Map).ToList();

           q.ShowInactive = false;

           return queryResult;

       }

       DiscountGroup Map(tblDiscountGroup tblDG)
       {
           DiscountGroup dg = new DiscountGroup(tblDG.id)
           {
                Name=tblDG.Name,
                 Code=tblDG.Code
           };
           dg._SetDateCreated(tblDG.IM_DateCreated);
           dg._SetDateLastUpdated(tblDG.IM_DateLastUpdated);
           dg._SetStatus((EntityStatus)tblDG.IM_Status);
           return dg;
       }
       
    }
}
