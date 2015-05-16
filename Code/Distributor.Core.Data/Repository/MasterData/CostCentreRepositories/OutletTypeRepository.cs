using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Domain.Master;
using Distributr.Core.Utility;
using log4net;
using Distributr.Core.Repository.Master;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;


namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class OutletTypeRepository : RepositoryMasterBase<OutletType>, IOutletTypeRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;

        public OutletTypeRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _log.Debug("Outlet Type Repository Constructor Bootstrap");
        }


        public Guid Save(OutletType entity, bool? isSync = null)
        {
            _log.Debug("Saving Outlet Type");

            //validation
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                _log.Debug(CoreResourceHelper.GetText("hq.outlettype.validation.error"));
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("hq.outlettype.validation.error"));
            }
            tblOutletType ot = _ctx.tblOutletType.FirstOrDefault(n => n.id == entity.Id);
            DateTime dt = DateTime.Now;
            if (ot == null)
            {

                ot = new tblOutletType();
                ot.IM_Status = (int)EntityStatus.Active;// true;
                ot.IM_DateCreated = dt;
                ot.id = entity.Id;
                _ctx.tblOutletType.AddObject(ot);
            }
            else
            {
               

            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (ot.IM_Status != (int)entityStatus)
                ot.IM_Status = (int)entity._Status;
            ot.Name = entity.Name;
            ot.Code = entity.Code;
            ot.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblOutletType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, ot.id));

            return ot.id;
        }

        public void SetInactive(OutletType entity)
        {
            _log.Debug("Inactivating Outlet Type");
            tblOutletType outletType = _ctx.tblOutletType.FirstOrDefault(p => p.id == entity.Id);
            if (outletType != null)
            {
                outletType.IM_Status = (int)EntityStatus.Inactive;// false;
                outletType.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
            }
            _cacheProvider.Put(_cacheListKey, _ctx.tblOutletType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, outletType.id));
           
        }

        public void SetActive(OutletType entity)
        {
            _log.Debug("Activating Outlet Type");
            tblOutletType outletType = _ctx.tblOutletType.FirstOrDefault(p => p.id == entity.Id);
            if (outletType != null)
            {
                outletType.IM_Status = (int) EntityStatus.Active;
                outletType.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
            }
            _cacheProvider.Put(_cacheListKey, _ctx.tblOutletType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, outletType.id));
        }

        public void SetAsDeleted(OutletType entity)
        {
            var vri = Validate(entity);
           bool hasDependency = _ctx.tblCostCentre
              .Where(n => n.CostCentreType == (int)CostCentreType.Outlet
                          && n.IM_Status != (int)EntityStatus.Deleted)
              .Any(n => n.Outlet_Type_Id == entity.Id);
           if (hasDependency)
               throw new DomainValidationException(vri, "Cannot delete - has a distributor assigned to the region.");
           _log.Debug("Deleting Outlet Type");
            tblOutletType ot = _ctx.tblOutletType.FirstOrDefault(p => p.id == entity.Id);
            if (ot != null)
            {
                ot.IM_Status = (int) EntityStatus.Deleted;
                ot.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
            }
            _cacheProvider.Put(_cacheListKey, _ctx.tblOutletType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s =>s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, ot.id));

        }

        public OutletType GetById(Guid Id, bool includeDeactivated = false)
        {
            _log.Debug("Getting Outlet Type by ID: " + Id);
            OutletType entity = (OutletType)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblOutletType.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = tbl.Map();
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        protected override string _cacheKey
        {
            get { return "OutletType-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "OutletTypeList"; }
        }

        public override IEnumerable<OutletType> GetAll(bool includeDeactivated = false)
        {
            IList<OutletType> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<OutletType>(ids.Count);
                foreach (Guid id in ids)
                {
                    OutletType entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblOutletType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => s.Map()).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (OutletType p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
            
            
        }

        public OutletType GetByName(string name, bool includeDeactivated = false)
        {
            return GetAll(includeDeactivated).FirstOrDefault(p => p.Name != null && p.Name.ToLower() == name.ToLower());
        }

        public QueryResult<OutletType> QueryResult(QueryStandard q)
        {
            IQueryable<tblOutletType> outletTypeQuery;

            if (q.ShowInactive)
                outletTypeQuery = _ctx.tblOutletType.Where(p => p.IM_Status != (int) EntityStatus.Deleted).AsQueryable();
            else
                outletTypeQuery = _ctx.tblOutletType.Where(p => p.IM_Status == (int)EntityStatus.Active).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q.Name))
                outletTypeQuery = outletTypeQuery.Where(p => p.Name.ToLower().Contains(q.Name.ToLower()));

            outletTypeQuery = outletTypeQuery.OrderBy(l => l.Name);

            var queryResult = new QueryResult<OutletType>();
            queryResult.Count = outletTypeQuery.Count();

            if (q.Skip.HasValue && q.Take.HasValue)
                outletTypeQuery = outletTypeQuery.Skip(q.Skip.Value).Take(q.Take.Value);

            var results = outletTypeQuery.ToList();

            queryResult.Data = results.ToList().Select(s => s.Map()).ToList();

            q.ShowInactive = false;

            return queryResult;
        }

        public ValidationResultInfo Validate(OutletType itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            bool hasDuplicate = GetAll(true).Where(s => s.Id != itemToValidate.Id).Any(n => n.Name == itemToValidate.Name);
            if (hasDuplicate)
            {
                vri.Results.Add(new ValidationResult(CoreResourceHelper.GetText("hq.outlettype.validation.dupname")));
            }
            return vri;
            
        }         
       

       
        }
       

}
