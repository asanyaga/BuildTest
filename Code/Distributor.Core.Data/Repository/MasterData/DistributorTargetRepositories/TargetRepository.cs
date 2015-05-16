using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility;
using log4net;
using Distributr.Core.Repository.Master.DistributorTargetRepositories;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.DistributorTargetRepositories
{
    internal class TargetRepository : RepositoryMasterBase<Target>, ITargetRepository
    {
       CokeDataContext _ctx;
       ICostCentreRepository _costCentreRepository;
       IProductRepository _productRepository;
       ITargetPeriodRepository _targetPeriodRepository;
       ICacheProvider _cacheProvider;
       public TargetRepository(CokeDataContext ctx,ICostCentreRepository costCentreRepository,IProductRepository productRepository,ITargetPeriodRepository targetPeriodRepository,ICacheProvider cacheProvider)
       {
           _ctx = ctx;
           _productRepository = productRepository;
           _targetPeriodRepository = targetPeriodRepository;
           _costCentreRepository = costCentreRepository;
           _cacheProvider = cacheProvider;
       }
       public Guid Save(Target entity, bool? isSync = null)
        {
            _log.DebugFormat("Saving/Updating Co");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            DateTime dt = DateTime.Now;
            if(!vri.IsValid)
            {
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("hq.targets.validation.error"));
            }
            tblTarget tblTarget = _ctx.tblTarget.FirstOrDefault(n => n.id == entity.Id);
            bool isNew = false;
            if (tblTarget == null)
            {
                isNew = true;
                tblTarget = new tblTarget();
                tblTarget.IM_DateCreated = dt;
                tblTarget.IM_Status = (int)EntityStatus.Active;// true;
                tblTarget.id = entity.Id;
                _ctx.tblTarget.AddObject(tblTarget);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tblTarget.IM_Status != (int)entityStatus)
                tblTarget.IM_Status = (int)entity._Status;
         
          tblTarget.CostCentreId = _costCentreRepository.GetById(entity.CostCentre.Id).Id;
            tblTarget.PeriodId = _targetPeriodRepository.GetById(entity.TargetPeriod.Id).Id;
            tblTarget.TargetValue = entity.TargetValue;
            tblTarget.IsQuantityTarget = entity.IsQuantityTarget;
            if (!isNew)
                tblTarget.IM_Status = (int)entity._Status;
            tblTarget.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblTarget.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tblTarget.id));
            return tblTarget.id;

        }

        public void SetActive(Target entity)
        {
            _log.DebugFormat("Activating target of Id " + entity.Id);
            ValidationResultInfo vri = Validate(entity);
            if (!vri.IsValid)
            {
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("hq.targets.validation.error"));
            }

            tblTarget target = _ctx.tblTarget.FirstOrDefault(n => n.id == entity.Id);
            if (target != null)
            {
                target.IM_Status = (int) EntityStatus.Active;
                target.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblTarget.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, target.id));
            }
        }

        public void SetInactive(Target entity)
        {
            _log.DebugFormat("Deactivating target of Id " + entity.Id);
            tblTarget target = _ctx.tblTarget.FirstOrDefault(n => n.id == entity.Id);
            if (target != null)
            {
                target.IM_Status = (int)EntityStatus.Inactive;//false;
                target.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblTarget.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, target.id));
            }
        }

        public void SetAsDeleted(Target entity)
        {
            var vri = Validate(entity);
            bool hasDependency = _ctx.tblCostCentre
                .Where(n => n.CostCentreType == (int)CostCentreType.Distributor
                            && n.IM_Status != (int)EntityStatus.Deleted)
                .SelectMany(n => n.tblTarget)
                .Any(n => n.CostCentreId == entity.CostCentre.Id);
            if (hasDependency)
                throw new DomainValidationException(vri, "Cannot delete " + entity.TargetPeriod.Name + "- has a distributor using it.");
            
            _log.DebugFormat("Deleting target of Id " + entity.Id);
            tblTarget target = _ctx.tblTarget.FirstOrDefault(n => n.id == entity.Id);
            if (target != null)
            {
                target.IM_Status = (int)EntityStatus.Deleted;//false;
                target.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblTarget.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, target.id));
            }
        }

        public Target GetById(Guid Id, bool includeDeactivated = false)
        {
            Target entity = (Target)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblTarget.FirstOrDefault(s => s.id == Id);
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
            get { return "Target-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "TargetList"; }
        }

        public override IEnumerable<Target> GetAll(bool includeDeactivated = false)
        {
            _log.DebugFormat("Getting All");
            IList<Target> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<Target>(ids.Count);
                foreach (Guid id in ids)
                {
                    Target entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblTarget.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (Target p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public QueryResult<Target> Query(QueryStandard q)
        {
            IQueryable<tblTarget> targetQuery;

            if (q.ShowInactive)
                targetQuery =
                    _ctx.tblTarget.Where(
                        s => s.IM_Status == (int) EntityStatus.Deleted || s.IM_Status == (int) EntityStatus.Inactive)
                        .AsQueryable();
            else
                targetQuery = _ctx.tblTarget.Where(s => s.IM_Status == (int) EntityStatus.Active).AsQueryable();
            var queryResult = new QueryResult<Target>();
            if (!string.IsNullOrWhiteSpace(q.Name))
            {
                targetQuery = targetQuery.Where(s => s.tblCostCentre.Name.ToLower().Contains(q.Name.ToLower()));
            }

            targetQuery = targetQuery.OrderBy(s => s.PeriodId);
            queryResult.Count = targetQuery.Count();
            
            if (q.Skip.HasValue && q.Take.HasValue)
                targetQuery = targetQuery.Skip(q.Skip.Value).Take(q.Take.Value);
            
            queryResult.Data = targetQuery.Select(Map).OfType<Target>().ToList();
            q.ShowInactive = false;
            
            return queryResult;
        }

         protected Target Map(tblTarget tblTarget)
        {
           Target trgt =new Target(tblTarget.id) 
            {
             TargetPeriod=_targetPeriodRepository.GetById(tblTarget.PeriodId),
             CostCentre=_costCentreRepository.GetById(tblTarget.CostCentreId),
               TargetValue=tblTarget.TargetValue,
               IsQuantityTarget=tblTarget.IsQuantityTarget
            };
           trgt._SetDateCreated(tblTarget.IM_DateCreated);
           trgt._SetDateLastUpdated(tblTarget.IM_DateLastUpdated);
           trgt._SetStatus((EntityStatus)tblTarget.IM_Status);
           return trgt;
        }
        public ValidationResultInfo Validate(Target itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            bool hasDuplicateCostCentre = GetAll(true)
                .Where(t => t.Id != itemToValidate.Id)
                .Any(c => c.CostCentre.Id == itemToValidate.CostCentre.Id);
            //if (hasDuplicateCostCentre)
            //    vri.Results.Add(new ValidationResult("Duplicate Distributor found"));
            bool hasDuplicatePeriod = GetAll(true)
                .Where(t => t.Id != itemToValidate.Id)
                .Any(p => p.TargetPeriod.Id == itemToValidate.TargetPeriod.Id);
            bool hasDuplicateDistributorPeriod = GetAll(true)
                .Where(d => d.Id != itemToValidate.Id)
                .Any(d => d.TargetPeriod.Id == itemToValidate.TargetPeriod.Id && 
                          d.CostCentre.Id == itemToValidate.CostCentre.Id);
            if (hasDuplicateDistributorPeriod)
                vri.Results.Add(new ValidationResult(CoreResourceHelper.GetText("hq.targets.validation.dupperiod")));
            return vri;
        }

        //public bool GetItemUpdatedSinceDateTime(DateTime dateTime)
        //{
        //    throw new NotImplementedException();
        //}

        //public DateTime GetLastTimeItemUpdated()
        //{
        //    throw new NotImplementedException();
        //}

       
    }
}
