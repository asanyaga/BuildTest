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
    internal class OutletVisitDayRepository : RepositoryMasterBase<OutletVisitDay>, IOutletVisitDayRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;

        public OutletVisitDayRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
        }



        public Guid Save(OutletVisitDay entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            } 
           
            if (!vri.IsValid)
            {
                _log.Debug("OutletVisitDay  not valid");
                throw new DomainValidationException(vri, "OutletVisitDay Entity Not valid");
            }
            DateTime dt = DateTime.Now;

            tblOutletVisitDay priority = _ctx.tblOutletVisitDay.FirstOrDefault(n => n.VistDay ==(int) entity.Day &&  n.OutletId==entity.Outlet.Id );
            if (priority == null)
            {
                priority = new tblOutletVisitDay();
                priority.IM_Status = (int)EntityStatus.Active;// true;
                priority.IM_DateCreated = dt;
                priority.Id = entity.Id;
                _ctx.tblOutletVisitDay.AddObject(priority);

            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (priority.IM_Status != (int)entityStatus)
                priority.IM_Status = (int)entity._Status;
            priority.EffectiveDate = entity.EffectiveDate;
            priority.OutletId = entity.Outlet.Id;
            priority.VistDay =(int) entity.Day;
            priority.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblOutletVisitDay.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, priority.Id));
            return priority.Id; 
        }

        public void SetInactive(OutletVisitDay entity)
        {
            tblOutletVisitDay outlet = _ctx.tblOutletVisitDay.FirstOrDefault(n => n.Id == entity.Id);
            if (outlet != null)
            {
                outlet.IM_Status = (int)EntityStatus.Inactive;// false;
                outlet.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblOutletVisitDay.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, outlet.Id));
            }
          
        }

        public void SetActive(OutletVisitDay entity)
        {
        	tblOutletVisitDay visitDay = _ctx.tblOutletVisitDay.FirstOrDefault(n => n.Id == entity.Id);
			if(visitDay != null)
			{
				visitDay.IM_Status = (int) EntityStatus.Active;
				visitDay.IM_DateLastUpdated = DateTime.Now;
				_ctx.SaveChanges();
				_cacheProvider.Put(_cacheListKey, _ctx.tblOutletVisitDay.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(n => n.Id).ToList());
				_cacheProvider.Remove(string.Format(_cacheKey, visitDay.Id));
			}
        }

        public void SetAsDeleted(OutletVisitDay entity)
        {
            tblOutletVisitDay outlet = _ctx.tblOutletVisitDay.FirstOrDefault(n => n.Id == entity.Id);
            if (outlet != null)
            {
                outlet.IM_Status = (int)EntityStatus.Deleted;// false;
                outlet.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblOutletVisitDay.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, outlet.Id));
            }
        }

        public OutletVisitDay GetById(Guid Id, bool includeDeactivated=false)
        {
            OutletVisitDay entity = (OutletVisitDay)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblOutletVisitDay.FirstOrDefault(s => s.Id == Id);
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
            get { return "OutletVisitDay-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "OutletVisitDayList"; }
        }

        public override IEnumerable<OutletVisitDay> GetAll(bool includeDeactivated)
        {
            IList<OutletVisitDay> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<OutletVisitDay>(ids.Count);
                foreach (Guid id in ids)
                {
                    OutletVisitDay entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblOutletVisitDay.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (OutletVisitDay p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        private OutletVisitDay Map(tblOutletVisitDay tbloutletvisit)
        {
            OutletVisitDay priority = new OutletVisitDay(tbloutletvisit.Id)
            {
                Outlet = new CostCentreRef { Id = tbloutletvisit.OutletId },
                Day =(DayOfWeek) tbloutletvisit.VistDay,
                EffectiveDate = tbloutletvisit.EffectiveDate,
            };

            priority._SetDateCreated(tbloutletvisit.IM_DateCreated);
            priority._SetDateLastUpdated(tbloutletvisit.IM_DateLastUpdated);
            priority._SetStatus((EntityStatus)tbloutletvisit.IM_Status);
            return priority;
        }

        public ValidationResultInfo Validate(OutletVisitDay itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            return vri;
        }
    }
}
