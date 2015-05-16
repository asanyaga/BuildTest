using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master;
using Distributr.Core.Utility;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class OutletCategoryRepository : RepositoryMasterBase<OutletCategory>, IOutletCategoryRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        public OutletCategoryRepository(CokeDataContext ctx, ICacheProvider CacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = CacheProvider;
            _log.Debug("Outlet Category Repository Constructor Bootstrap");
        }

        public Guid Save(OutletCategory entity, bool? isSync = null)
        {
            _log.Debug("Saving an Outlet Category");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("hq.outletcat.validation.error"));
            tblOutletCategory tbloutletcategory = _ctx.tblOutletCategory.FirstOrDefault(n => n.id == entity.Id);
            DateTime dt = DateTime.Now;
            if (tbloutletcategory == null)
            {

                tbloutletcategory = new tblOutletCategory();
                tbloutletcategory.id = entity.Id;
                tbloutletcategory.IM_Status = (int)EntityStatus.Active;// true;
                tbloutletcategory.IM_DateCreated = dt;
                _ctx.tblOutletCategory.AddObject(tbloutletcategory);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tbloutletcategory.IM_Status != (int)entityStatus)
                tbloutletcategory.IM_Status = (int)entity._Status;
            tbloutletcategory.Name = entity.Name;
            tbloutletcategory.Code = entity.Code;
            tbloutletcategory.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblOutletCategory.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tbloutletcategory.id));
          
            return tbloutletcategory.id;
        }

        public void SetAsDeleted(OutletCategory entity)
        {
            ValidationResultInfo vri = Validate(entity);
            _log.Debug("Deleting Outlet Category");
            bool hasDependency = false;
            hasDependency = _ctx.tblCostCentre.Where(n => n.Outlet_Category_Id.Value == entity.Id) 
                .Any(n => n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive);
            if (hasDependency)
            {
                throw new DomainValidationException(vri, "Cannot Delete Outlet Category\r\n Dependency Found");
            }
           _log.Debug("Deleting an Outlet Category ID: " + entity.Id);
            tblOutletCategory oc = _ctx.tblOutletCategory.FirstOrDefault(n => n.id == entity.Id);
            if (oc != null)
            {
                oc.IM_Status = (int) EntityStatus.Deleted;
                oc.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblOutletCategory.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, oc.id));
            }
        }

        public OutletCategory GetById(Guid Id, bool includeDeactivated = false)
        {
            _log.Debug("Getting Outlet Category by ID: " + Id);
            OutletCategory entity = (OutletCategory)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblOutletCategory.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity =tbl.Map();
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        protected override string _cacheKey
        {
            get { return "OutletCategory-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "OutletCategoryList"; }
        }

        public override IEnumerable<OutletCategory> GetAll(bool includeDeactivated = false)
        {
            _log.Debug("Getting All Outlet Categories; include Deactivated: " + includeDeactivated);
            IList<OutletCategory> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<OutletCategory>(ids.Count);
                foreach (Guid id in ids)
                {
                    OutletCategory entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblOutletCategory.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => s.Map()).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (OutletCategory p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public OutletCategory GetByName(string name, bool includeDeactivated = false)
        {
            return GetAll(includeDeactivated).FirstOrDefault(p => p.Name != null && p.Name.ToLower() == name.ToLower());
        }

        public QueryResult<OutletCategory> Query(QueryStandard q)
        {
            IQueryable<tblOutletCategory> outletCartegoryQuery;

            if (q.ShowInactive)
                outletCartegoryQuery =
                    _ctx.tblOutletCategory.Where(n => n.IM_Status != (int) EntityStatus.Deleted).AsQueryable();

            else
                outletCartegoryQuery =
                    _ctx.tblOutletCategory.Where(n => n.IM_Status == (int) EntityStatus.Active).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q.Name))
                outletCartegoryQuery = outletCartegoryQuery.Where(n => n.Name.ToLower().Contains(q.Name));

            outletCartegoryQuery = outletCartegoryQuery.OrderBy(n => n.Name);

            var queryResult = new QueryResult<OutletCategory>();
            queryResult.Count = outletCartegoryQuery.Count();

            if (q.Take.HasValue && q.Skip.HasValue)
                outletCartegoryQuery = outletCartegoryQuery.Skip(q.Skip.Value).Take(q.Take.Value);

            var result = outletCartegoryQuery.ToList();

            queryResult.Data = result.ToList().Select(s => s.Map()).ToList();

            q.ShowInactive = false;

            return queryResult;
        }

        public void SetInactive(OutletCategory entity)
        {
            _log.Debug("Deactivating an Outlet Category ID: " + entity.Id);
            ValidationResultInfo vri = Validate(entity);
            bool hasDependency = false;
            hasDependency = _ctx.tblCostCentre.Where(n => n.Outlet_Category_Id.Value == entity.Id)
                .Any(n => n.IM_Status == (int)EntityStatus.Active || n.IM_Status == (int)EntityStatus.Inactive);
            if (hasDependency)
            {
                throw new DomainValidationException(vri, "Cannot Deactivate Outlet Category\r\n Dependency Found");
            }
            tblOutletCategory tbloutletcategory = _ctx.tblOutletCategory.FirstOrDefault(n => n.id == entity.Id);
            if (tbloutletcategory != null)
            {
                tbloutletcategory.IM_Status = (int)EntityStatus.Inactive;// false;
                tbloutletcategory.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblOutletCategory.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tbloutletcategory.id));
            }
           
        }

        public void SetActive(OutletCategory entity)
        {
            _log.Debug("Activating an Outlet Category ID: " + entity.Id);
            tblOutletCategory tbloutletcategory = _ctx.tblOutletCategory.FirstOrDefault(n => n.id == entity.Id);
            if (tbloutletcategory != null)
            {
                tbloutletcategory.IM_Status = (int)EntityStatus.Active;// false;
                tbloutletcategory.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblOutletCategory.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tbloutletcategory.id));
            }
        }


        public ValidationResultInfo Validate(OutletCategory itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            bool hasDuplicate =_ctx.tblOutletCategory
                .Where(n => n.id != itemToValidate.Id)
                .Any(n => n.Name.ToLower() == itemToValidate.Name.ToLower()); 
            if (hasDuplicate)
            {
                vri.Results.Add(new ValidationResult(CoreResourceHelper.GetText("hq.outletcat.validation.dupname")));
            }
            
            bool hasDuplicatecode = _ctx.tblOutletCategory
                .Where(n => n.id != itemToValidate.Id)
                .Any(n => n.Code.ToLower() == itemToValidate.Code.ToLower());
            if (hasDuplicatecode)
            {
                vri.Results.Add(new ValidationResult(CoreResourceHelper.GetText("hq.outletcat.validation.dupcode")));
            }
            return vri;
        }

       
    }
}
