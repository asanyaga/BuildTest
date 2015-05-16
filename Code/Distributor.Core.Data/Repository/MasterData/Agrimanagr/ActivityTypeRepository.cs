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
    public class ActivityTypeRepository:RepositoryMasterBase<ActivityType>,IActivityTypeRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;

        public ActivityTypeRepository(CokeDataContext ctx, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
        }

        

        public Guid Save(ActivityType entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }

            if (!vri.IsValid)
            {
                _log.Debug(CoreResourceHelper.GetText("Activity.validation.error"));
                throw new DomainValidationException(vri, "");
            }
            DateTime dt = DateTime.Now;

            tblActivityType activityType = _ctx.tblActivityType.FirstOrDefault(n => n.Id == entity.Id);
            if (activityType == null)
            {
                activityType = new tblActivityType();
                activityType.Id = entity.Id;
                activityType.IM_Status = (int)EntityStatus.Active;
                activityType.IM_DateCreated = dt;
                _ctx.tblActivityType.AddObject(activityType);

            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (activityType.IM_Status != (int)entityStatus)
                activityType.IM_Status = (int)entity._Status;
            activityType.Code = entity.Code;
            activityType.Name = entity.Name;
            activityType.IsInfectionsRequired = entity.IsInfectionsRequired;
            activityType.IsInputsRequired = entity.IsInputRequired;
            activityType.IsProduceRequired = entity.IsProduceRequired;
            activityType.IsServicesRequired = entity.IsServicesRequired;
            activityType.Description = entity.Description;
            activityType.IM_DateLastUpdated = dt;
          
            
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey,_ctx.tblActivityType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, activityType.Id));
            return activityType.Id; 
        }

        public void SetInactive(ActivityType entity)
        {
            var service = _ctx.tblActivityType.FirstOrDefault(n => n.Id == entity.Id);
            if (service != null)
            {
                service.IM_Status = (int)EntityStatus.Inactive;
                service.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblActivityType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, service.Id));
            }
        }

        public void SetActive(ActivityType entity)
        {
            var service = _ctx.tblActivityType.FirstOrDefault(n => n.Id == entity.Id);
            if (service != null)
            {
                service.IM_Status = (int)EntityStatus.Active;
                service.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblActivityType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, service.Id));
            }
        }

        public void SetAsDeleted(ActivityType entity)
        {
            var service = _ctx.tblActivityType.FirstOrDefault(n => n.Id == entity.Id);
            if (service != null)
            {
                service.IM_Status = (int)EntityStatus.Deleted;
                service.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblActivityType.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, service.Id));
            }
        }

        public ActivityType GetById(Guid Id, bool includeDeactivated = false)
        {
            ActivityType entity = (ActivityType)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblActivityType.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public override IEnumerable<ActivityType> GetAll(bool includeDeactivated = false)
        {
           throw new NotImplementedException();
        }

      

        public QueryResult<ActivityType> Query(QueryBase query)
        {
            var q = query as QueryActivityType;
            IQueryable<tblActivityType> activityTypes=_ctx.tblActivityType;

            activityTypes = q.ShowInactive
                                ? activityTypes.Where(p => p.IM_Status != (int) EntityStatus.Deleted).AsQueryable()
                                : activityTypes.Where(s => s.IM_Status == (int) EntityStatus.Active).AsQueryable();
            var queryResult = new QueryResult<ActivityType>();
            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                activityTypes = activityTypes
                    .Where(s => s.Name.ToLower().Contains(q.Name.ToLower()) || s.Code.ToLower().Contains(q.Name.ToLower()));
            }

            queryResult.Count = activityTypes.Count();
            activityTypes = activityTypes.OrderBy(s => s.Name).ThenBy(s => s.Code);
            if (q.Skip.HasValue && q.Take.HasValue)
                activityTypes = activityTypes.Skip(q.Skip.Value).Take(q.Take.Value);
            var result = activityTypes.ToList();
            queryResult.Data = result.Select(Map).OfType<ActivityType>().ToList();
          
            return queryResult;
        }

        public ValidationResultInfo Validate(ActivityType itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;

            var query = _ctx.tblActivityType.Where(p => p.IM_Status != (int)EntityStatus.Deleted);
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));

            if (query.Any(s => s.Id!= itemToValidate.Id && !string.IsNullOrEmpty(s.Code) && s.Code == itemToValidate.Code))
                vri.Results.Add(new ValidationResult("Duplicate Code found"));

            if (query.Any(s => s.Id != itemToValidate.Id && s.Name == itemToValidate.Name))
                vri.Results.Add(new ValidationResult("Duplicate Name found"));

            return vri;
        }

        public ActivityType Map(tblActivityType s)
        {
            var service = new ActivityType(s.Id)
            {
                Code = (s.Code ?? ""),
                Description = s.Description??"",
                Name = s.Name,
               
              IsInfectionsRequired = s.IsInfectionsRequired,
              IsInputRequired = s.IsInputsRequired,
              IsProduceRequired = s.IsProduceRequired,
              IsServicesRequired =s.IsServicesRequired,
              _DateCreated = s.IM_DateCreated,
              _DateLastUpdated = s.IM_DateLastUpdated,
              
                
            };
            service._SetDateCreated(s.IM_DateCreated);
            service._SetDateLastUpdated(s.IM_DateLastUpdated);
            service._SetStatus((EntityStatus)s.IM_Status);
            return service;
        }
        protected override string _cacheKey
        {
            get { return "ActivityType-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "ActivityTypeList"; }
        }
    }
}
