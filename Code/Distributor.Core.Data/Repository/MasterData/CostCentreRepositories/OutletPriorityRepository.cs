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
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.CostCentreRepositories
{
    internal class OutletPriorityRepository : RepositoryMasterBase<OutletPriority>, IOutletPriorityRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        private IRouteRepository _routeRepository;

        public OutletPriorityRepository(CokeDataContext ctx, ICacheProvider cacheProvider, IRouteRepository routeRepository)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            _routeRepository = routeRepository;
        }



        public Guid Save(OutletPriority entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            } 
           
            if (!vri.IsValid)
            {
                _log.Debug("OutletPriority  not valid");
                throw new DomainValidationException(vri, "OutletPriority Entity Not valid");
            }
            DateTime dt = DateTime.Now;
            var priority = _ctx.tblOutletPriority.FirstOrDefault(n => n.OutletId == entity.Outlet.Id && n.RouteId == entity.Route.Id);
            if (priority == null)
            {
                priority = new tblOutletPriority();
                priority.IM_Status = (int)EntityStatus.Active;// true;
                priority.IM_DateCreated = dt;
                priority.Id = entity.Id;
                _ctx.tblOutletPriority.AddObject(priority);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (priority.IM_Status != (int)entityStatus)
                priority.IM_Status = (int)entity._Status;
            priority.OutletId = entity.Outlet.Id;
            priority.OutletPriority = entity.Priority;
            priority.RouteId = entity.Route.Id;
            priority.EffectiveDate = entity.EffectiveDate;
            priority.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblOutletPriority.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, priority.Id));
            return priority.Id; 
        }

        public void SetInactive(OutletPriority entity)
        {

            tblOutletPriority outlet = _ctx.tblOutletPriority.FirstOrDefault(n => n.Id == entity.Id);
            if (outlet != null)
            {
                outlet.IM_Status = (int)EntityStatus.Inactive;//false;
                outlet.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblOutletPriority.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, outlet.Id));

            }
           
        }

        public void SetActive(OutletPriority entity)
        {
        	tblOutletPriority outletPriority = _ctx.tblOutletPriority.FirstOrDefault(n => n.Id == entity.Id);
			if (outletPriority != null)
			{
				outletPriority.IM_Status = (int) EntityStatus.Active;
				outletPriority.IM_DateLastUpdated = DateTime.Now;
				_ctx.SaveChanges();
				_cacheProvider.Put(_cacheListKey, _ctx.tblOutletPriority.Where(n=>n.IM_Status != (int)EntityStatus.Deleted).Select(n=>n.Id).ToList());
				_cacheProvider.Remove(string.Format(_cacheKey, outletPriority.Id));
			}
        }

        public void SetAsDeleted(OutletPriority entity)
        {
            tblOutletPriority outlet = _ctx.tblOutletPriority.FirstOrDefault(n => n.Id == entity.Id);
            if (outlet != null)
            {
                outlet.IM_Status = (int)EntityStatus.Deleted;//false;
                outlet.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblOutletPriority.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, outlet.Id));

            }
        }

        public OutletPriority GetById(Guid Id, bool includeDeactivated=false)
        {
            OutletPriority entity = (OutletPriority)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblOutletPriority.FirstOrDefault(s => s.Id == Id);
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
            get { return "OutletPriority-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "OutletPriorityList"; }
        }

        public override IEnumerable<OutletPriority> GetAll(bool includeDeactivated)
        {
            IList<OutletPriority> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<OutletPriority>(ids.Count);
                foreach (Guid id in ids)
                {
                    OutletPriority entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblOutletPriority.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (OutletPriority p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public ValidationResultInfo Validate(OutletPriority itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            return vri;
        }
        public OutletPriority Map(tblOutletPriority tblpriority)
        {
            OutletPriority priority = new OutletPriority(tblpriority.Id)
                                          {
                                              Outlet = new CostCentreRef {Id = tblpriority.OutletId},
                                              Route = _routeRepository.GetById(tblpriority.RouteId),
                                              Priority = tblpriority.OutletPriority,
                                              EffectiveDate = tblpriority.EffectiveDate,
            };
            priority._SetDateCreated(tblpriority.IM_DateCreated);
            priority._SetDateLastUpdated(tblpriority.IM_DateLastUpdated);
            priority._SetStatus((EntityStatus)tblpriority.IM_Status);
            return priority;
        }
    }
}
