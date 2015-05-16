using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Repository.Master.DistributorTargetRepositories;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.IOC;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Data.Utility.Caching;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.DistributorTargetRepositories
{
   internal class TargetPeriodRepository:RepositoryMasterBase<TargetPeriod >,ITargetPeriodRepository 
    {
       CokeDataContext _ctx;
       ICostCentreRepository _costCentreRepository;
       ICacheProvider _cacheProvider;
       public TargetPeriodRepository(CokeDataContext ctx,  ICostCentreRepository costCentreRepository, ICacheProvider cacheProvider)
       {
           _ctx = ctx;
           _costCentreRepository = costCentreRepository;
           _cacheProvider = cacheProvider;
           _log.Debug("TargetPeriod Repository Constructor BootStrap!");
       }


       protected override string _cacheKey
       {
           get { return "TargetPeriod-{0}"; }
       }

       protected override string _cacheListKey
       {
           get { return "TargetPeriodList"; }
       }

       public override IEnumerable<TargetPeriod> GetAll(bool includeDeactivated = false)
        {
            IList<TargetPeriod> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<TargetPeriod>(ids.Count);
                foreach (Guid id in ids)
                {
                    TargetPeriod entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblTargetPeriod.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (TargetPeriod p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

       public QueryResult<TargetPeriod> Query(QueryStandard q)
       {
           IQueryable<tblTargetPeriod> targetPeriodResult;

           if (q.ShowInactive)
               targetPeriodResult =
                   _ctx.tblTargetPeriod.Where(
                       s => s.IM_Status != (int) EntityStatus.Deleted).AsQueryable();
           else
               targetPeriodResult = _ctx.tblTargetPeriod.Where(s => s.IM_Status == (int) EntityStatus.Active).AsQueryable();

           var queryResult = new QueryResult<TargetPeriod>();
           
           if (!string.IsNullOrWhiteSpace(q.Name))
               targetPeriodResult = targetPeriodResult.Where(s => s.Name.ToLower().Contains(q.Name.ToLower()));

           queryResult.Count = targetPeriodResult.Count();
           targetPeriodResult = targetPeriodResult.OrderBy(s => s.Name).ThenBy(s => s.StartDate);

           if (q.Take.HasValue && q.Skip.HasValue)
               targetPeriodResult = targetPeriodResult.Skip(q.Skip.Value).Take(q.Take.Value);
           
           var result = targetPeriodResult.ToList();
           queryResult.Data = result.Select(Map).ToList();
           
           return queryResult;
       }

       public Guid Save(TargetPeriod entity, bool? isSync = null)
        {
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            } 
           
            if (!vri.IsValid)
            {
                _log.Debug("TargetPeriod not valid");
                throw new DomainValidationException(vri, "TargetPeriod Entity Not valid");
            }
            DateTime dt = DateTime.Now;
            tblTargetPeriod targetPeriod = _ctx.tblTargetPeriod.FirstOrDefault(n => n.Id == entity.Id);
            if (targetPeriod == null)
            {
                targetPeriod = new tblTargetPeriod();
                targetPeriod.IM_Status = (int)EntityStatus.Active;// true;
                targetPeriod.IM_DateCreated = dt;
                targetPeriod.Id = entity.Id;
                
                _ctx.tblTargetPeriod.AddObject(targetPeriod);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (targetPeriod.IM_Status != (int)entityStatus)
                targetPeriod.IM_Status = (int)entity._Status;
            targetPeriod.Name = entity.Name;
            targetPeriod.StartDate = entity.StartDate;
            targetPeriod.EndDate = entity.EndDate;
            targetPeriod.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblTargetPeriod.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, targetPeriod.Id));
            return targetPeriod.Id;
        }

       public void SetActive(TargetPeriod entity)
       {
           ValidationResultInfo vri = Validate(entity);

           if (!vri.IsValid)
           {
               _log.Debug("TargetPeriod not valid");
               throw new DomainValidationException(vri, "TargetPeriod Entity Not valid");
           }

           tblTargetPeriod targetPeriod = _ctx.tblTargetPeriod.FirstOrDefault(n => n.Id == entity.Id);
           if (targetPeriod != null)
           {
               targetPeriod.IM_Status = (int)EntityStatus.Active; 
               targetPeriod.IM_DateLastUpdated = DateTime.Now;
               _ctx.SaveChanges();
               _cacheProvider.Put(_cacheListKey, _ctx.tblTargetPeriod.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
               _cacheProvider.Remove(string.Format(_cacheKey, targetPeriod.Id));
           }
       }

        public void SetInactive(TargetPeriod entity)
        {
            ValidationResultInfo vri = Validate(entity);
            bool hasTargetPeriodDependencies = _ctx.tblTarget.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.PeriodId== entity.Id);

            if (hasTargetPeriodDependencies)
            {
                throw new DomainValidationException(vri, "Cannot deactivate\r\nDependencies found");
            }
            else
            {
                tblTargetPeriod targetPeriod = _ctx.tblTargetPeriod.FirstOrDefault(n => n.Id == entity.Id);
                if (targetPeriod != null)
                {
                    targetPeriod.IM_Status = (int)EntityStatus.Inactive;// false;
                    targetPeriod.IM_DateLastUpdated = DateTime.Now;
                    _ctx.SaveChanges();
                    _cacheProvider.Put(_cacheListKey, _ctx.tblTargetPeriod.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                    _cacheProvider.Remove(string.Format(_cacheKey, targetPeriod.Id));

                }
            }
           
        }

       public void SetAsDeleted(TargetPeriod entity)
       {
           ValidationResultInfo vri = Validate(entity);
           bool hasTargetPeriodDependencies = _ctx.tblTarget.Where(s => s.IM_Status == (int)EntityStatus.Active).Any(p => p.PeriodId == entity.Id);

           if (hasTargetPeriodDependencies)
           {
               throw new DomainValidationException(vri, "Cannot Delete\r\nDependencies found");
           }
           else
           {
               tblTargetPeriod targetPeriod = _ctx.tblTargetPeriod.FirstOrDefault(n => n.Id == entity.Id);
               if (targetPeriod != null)
               {
                   targetPeriod.IM_Status = (int)EntityStatus.Deleted; 
                   targetPeriod.IM_DateLastUpdated = DateTime.Now;
                   _ctx.SaveChanges();
                   _cacheProvider.Put(_cacheListKey, _ctx.tblTargetPeriod.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                   _cacheProvider.Remove(string.Format(_cacheKey, targetPeriod.Id));
               }
           }
       }

       public TargetPeriod GetById(Guid Id, bool includeDeactivated = false)
        {
            TargetPeriod entity = (TargetPeriod)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblTargetPeriod.FirstOrDefault(s => s.Id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public ValidationResultInfo Validate(TargetPeriod itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            //start date should be before end date
            if(itemToValidate.StartDate > itemToValidate.EndDate)
                vri.Results.Add(new ValidationResult("Start date must be before end date"));

            //check for duplicate names
            bool hasDuplicateName = GetAll(true)
                .Where(s => s.Id != itemToValidate.Id)
                .Any(p => p.Name.ToLower() == itemToValidate.Name.ToLower());
            if (hasDuplicateName)
                vri.Results.Add(new ValidationResult("Duplicate Name found"));

            //check for dates within an existing period
            if (!hasDuplicateName)
            {
                if (DateAlreadyInPeriod(itemToValidate.StartDate, itemToValidate.Id))
                    vri.Results.Add(new ValidationResult("Start date already exists in an existing defined period"));
                if (DateAlreadyInPeriod(itemToValidate.EndDate, itemToValidate.Id))
                    vri.Results.Add(new ValidationResult("End date already exists in an existing defined period"));

                //check for periods within this period
                if (ExistsOtherPeriodWithinThisPeriod(itemToValidate))
                    vri.Results.Add(new ValidationResult("There exists another period within the defined period"));
            }
            return vri;
        }

        bool DateAlreadyInPeriod(DateTime dateTime, Guid idToValidate)
        {
            var dateAlreadyInPeriod = GetAll(true).Where(n => n.Id != idToValidate).Where(n => n.IsWithinDateRange(dateTime)).ToList();
            return dateAlreadyInPeriod.Any();
        }

        bool ExistsOtherPeriodWithinThisPeriod(TargetPeriod itemToValidate)
        {
            var periodsWithinThis = GetAll(true)
                .Where(n => n.Id != itemToValidate.Id)
                .Where(n =>
                    itemToValidate.IsWithinDateRange(n.StartDate) &&
                    itemToValidate.IsWithinDateRange(n.EndDate)
                );
            return periodsWithinThis.Any();
       }

       public TargetPeriod Map( tblTargetPeriod targetPeriod)
        {
            TargetPeriod period = new TargetPeriod(targetPeriod.Id)
            {
                Name = targetPeriod.Name,
                StartDate = targetPeriod.StartDate,
                EndDate = targetPeriod.EndDate,
            };
            period._SetDateCreated(targetPeriod.IM_DateCreated);
            period._SetDateLastUpdated(targetPeriod.IM_DateLastUpdated);
            period._SetStatus((EntityStatus)targetPeriod.IM_Status);

            return period;
        }
    }
}
