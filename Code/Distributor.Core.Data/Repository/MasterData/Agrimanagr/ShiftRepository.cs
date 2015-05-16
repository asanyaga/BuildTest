using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.FarmActivities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.Agrimanagr;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.Agrimanagr
{
    public class ShiftRepository:RepositoryMasterBase<Shift>,IShiftRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;

        public ShiftRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
            
        }

        public Guid Save(Shift entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }

            if (!vri.IsValid)
            {
                _log.Debug(CoreResourceHelper.GetText("Shift.validation.error"));
                throw new DomainValidationException(vri, "");
            }
            DateTime dt = DateTime.Now;

            tblShift shift = _ctx.tblShift.FirstOrDefault(n => n.id == entity.Id);
            if (shift == null)
            {
                shift = new tblShift();
                shift.id = entity.Id;
                shift.IM_Status = (int)EntityStatus.Active;
                shift.IM_DateCreated = dt;
                _ctx.tblShift.AddObject(shift);

            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (shift.IM_Status != (int)entityStatus)
                shift.IM_Status = (int)entity._Status;
            shift.Code = entity.Code;
            shift.Name = entity.Name;
            shift.StartTime = entity.StartTime;
            shift.EndTime = entity.EndTime;
            shift.Description = entity.Description;
            shift.IM_DateLastUpdated = dt;
           
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblShift.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, shift.id));
            return shift.id; 
        }

        public void SetInactive(Shift entity)
        {
            var Shift = _ctx.tblShift.FirstOrDefault(n => n.id == entity.Id);
            if (Shift != null)
            {
                Shift.IM_Status = (int)EntityStatus.Inactive;
                Shift.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblShift.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, Shift.id));
            }
        }

        public void SetActive(Shift entity)
        {
            var Shift = _ctx.tblShift.FirstOrDefault(n => n.id == entity.Id);
            if (Shift != null)
            {
                Shift.IM_Status = (int)EntityStatus.Active;
                Shift.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblShift.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, Shift.id));
            }
        }

        public void SetAsDeleted(Shift entity)
        {
            var Shift = _ctx.tblShift.FirstOrDefault(n => n.id == entity.Id);
            if (Shift != null)
            {
                Shift.IM_Status = (int)EntityStatus.Deleted;
                Shift.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblShift.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, Shift.id));
            }
        }

        public Shift GetById(Guid Id, bool includeDeactivated = false)
        {
            Shift entity = (Shift)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblShift.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public override IEnumerable<Shift> GetAll(bool includeDeactivated = false)
        {
            IList<Shift> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<Shift>(ids.Count);
                foreach (Guid id in ids)
                {
                    Shift entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblShift.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(Map).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList();
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (Shift p in entities)
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

    public QueryResult<Shift> Query(QueryBase query)
        {
            var q = query as QueryShift;
            IQueryable<tblShift> shiftQuery;
            shiftQuery = q.ShowInactive
                             ? _ctx.tblShift.Where(p => p.IM_Status != (int) EntityStatus.Deleted).AsQueryable()
                             : _ctx.tblShift.Where(s => s.IM_Status == (int) EntityStatus.Active).AsQueryable();

            var queryResult = new QueryResult<Shift>();
            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                shiftQuery = shiftQuery
                    .Where(s => s.Name.ToLower().Contains(q.Name.ToLower()) || s.Code.ToLower().Contains(q.Name.ToLower()));
            }

            queryResult.Count = shiftQuery.Count();
            shiftQuery = shiftQuery.OrderBy(s => s.Name).ThenBy(s => s.Code);
            if (q.Skip.HasValue && q.Take.HasValue)
                shiftQuery = shiftQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = shiftQuery.ToList();
            queryResult.Data = result.Select(Map).OfType<Shift>().ToList();
            q.ShowInactive = false;
            return queryResult;
        }

        public ValidationResultInfo Validate(Shift itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;

            var query = _ctx.tblShift.Where(p => p.IM_Status != (int)EntityStatus.Deleted);
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            
            if (query.Any(s => s.id != itemToValidate.Id && !string.IsNullOrEmpty(s.Code) && s.Code == itemToValidate.Code))
                vri.Results.Add(new ValidationResult("Duplicate Code found"));

            if (query.Any(s => s.id != itemToValidate.Id && s.Name == itemToValidate.Name))
                vri.Results.Add(new ValidationResult("Duplicate Name found"));

            if (itemToValidate.StartTime>itemToValidate.EndTime)
                vri.Results.Add(new ValidationResult("End Time should be later than Start Time"));

            return vri;
        }

        public Shift Map(tblShift s)
        {
           
            var shift = new Shift(s.id)
            {
                Code = (s.Code ?? ""),
                Description = (s.Description??""),
                Name = s.Name,
                StartTime=s.StartTime,
                EndTime=s.EndTime,
               
            };
            shift._SetDateCreated(s.IM_DateCreated);
            shift._SetDateLastUpdated(s.IM_DateLastUpdated);
            shift._SetStatus((EntityStatus)s.IM_Status);
            return shift;
        }

        protected override string _cacheKey
        {
            get { return "Shift-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "ShiftList"; }
        }
    }
}
