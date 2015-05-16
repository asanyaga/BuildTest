using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.ReOrderLevelRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master.ReOrdeLevelEntities;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.ReOrderLevelRepository
{
   internal class ReOrderLevelRepository:RepositoryMasterBase<ReOrderLevel>,IReOrderLevelRepository
    {
       CokeDataContext _ctx;
       ICacheProvider _cacheProvider;
       ICostCentreRepository _costCentreRepository;
       IProductRepository _productRepository;
       public ReOrderLevelRepository(ICostCentreRepository costCentreRepository, IProductRepository productRepository,CokeDataContext ctx,ICacheProvider cacheProvider)
       {
           _costCentreRepository = costCentreRepository;
           _productRepository = productRepository;
           _ctx = ctx;
           _cacheProvider = cacheProvider;
       }
        public Guid Save(ReOrderLevel entity, bool? isSync = null)
        {
            _log.DebugFormat("Saving/Updating ReOrderLevel");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                throw new DomainValidationException(vri, CoreResourceHelper.GetText("hq.reorder.validation.error"));
            }
            DateTime dt = DateTime.Now;
            //tblReOrderLevel tblRLevel = _ctx.tblReOrderLevel.FirstOrDefault(n => n.id == entity.Id);
            //cn: make unique by product and distributor
            tblReOrderLevel tblRLevel =
                _ctx.tblReOrderLevel.FirstOrDefault(
                    n => n.DistributorId == entity.DistributorId.Id && n.ProductId == entity.ProductId.Id && n.IM_Status==(int)EntityStatus.Active);
            if (tblRLevel ==null)
            {
                tblRLevel = new tblReOrderLevel();
                tblRLevel.IM_DateCreated = dt;
                tblRLevel.IM_Status = (int)EntityStatus.Active;//true;
                tblRLevel.id = entity.Id;
                _ctx.tblReOrderLevel.AddObject(tblRLevel);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tblRLevel.IM_Status != (int)entityStatus)
                tblRLevel.IM_Status = (int)entity._Status;   
            tblRLevel.IM_DateLastUpdated = dt;
            tblRLevel.DistributorId = entity.DistributorId.Id;
            tblRLevel.ProductId = entity.ProductId.Id;
            tblRLevel.ProductReOrderLevel = entity.ProductReOrderLevel;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblReOrderLevel.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tblRLevel.id));
            return tblRLevel.id;
            
        }

        public void SetInactive(ReOrderLevel entity)
        {
            _log.DebugFormat("Deactivating ReOrderLevel");
            tblReOrderLevel tblRLevel = _ctx.tblReOrderLevel.FirstOrDefault(n => n.id == entity.Id);
            if (tblRLevel != null)
            {
                tblRLevel.IM_DateLastUpdated = DateTime.Now;
                tblRLevel.IM_Status = (int) EntityStatus.Inactive;// false;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblReOrderLevel.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblRLevel.id));
            }
        }

       public void SetActive(ReOrderLevel entity)
       {
           _log.DebugFormat("Activating ReOrderLevel");
           ValidationResultInfo vri = Validate(entity);
           if (!vri.IsValid)
           {
               throw new DomainValidationException(vri, CoreResourceHelper.GetText("hq.reorder.validation.error"));
           }
           tblReOrderLevel tblRLevel = _ctx.tblReOrderLevel.FirstOrDefault(n => n.id == entity.Id);
           if (tblRLevel != null)
           {
               tblRLevel.IM_DateLastUpdated = DateTime.Now;
               tblRLevel.IM_Status = (int)EntityStatus.Active;
               _ctx.SaveChanges();
               _cacheProvider.Put(_cacheListKey, _ctx.tblReOrderLevel.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
               _cacheProvider.Remove(string.Format(_cacheKey, tblRLevel.id));
           }
       }

       public void SetAsDeleted(ReOrderLevel entity)
       {
           _log.DebugFormat("Deleting ReOrderLevel");
           tblReOrderLevel tblRLevel = _ctx.tblReOrderLevel.FirstOrDefault(n => n.id == entity.Id);
           if (tblRLevel != null)
           {
               tblRLevel.IM_DateLastUpdated = DateTime.Now;
               tblRLevel.IM_Status = (int)EntityStatus.Deleted;// false;
               _ctx.SaveChanges();
               _cacheProvider.Put(_cacheListKey, _ctx.tblReOrderLevel.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
               _cacheProvider.Remove(string.Format(_cacheKey, tblRLevel.id));
           }
       }

       public ReOrderLevel GetById(Guid Id, bool includeDeactivated = false)
        {
            ReOrderLevel entity = (ReOrderLevel)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblReOrderLevel.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public ValidationResultInfo Validate(ReOrderLevel itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            bool hasDuplicateValue = GetAll(true)
                .Where(n => n.Id != itemToValidate.Id && n.DistributorId.Id == itemToValidate.DistributorId.Id 
                    && n._Status == EntityStatus.Active)
                .Any(n => n.ProductId.Id == itemToValidate.ProductId.Id);
            if (hasDuplicateValue)
            {
                vri.Results.Add(new ValidationResult(itemToValidate.ProductId.Description + " has an active re-order level present"));
            }
            return vri;
        }

       

       protected override string _cacheKey
       {
           get { return "ReOrderLevel-{0}"; }
       }

       protected override string _cacheListKey
       {
           get { return "ReOrderLevelList"; }
       }

       public override IEnumerable<ReOrderLevel> GetAll(bool includeDeactivated = false)
        {
            IList<ReOrderLevel> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<ReOrderLevel>(ids.Count);
                foreach (Guid id in ids)
                {
                    ReOrderLevel entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblReOrderLevel.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (ReOrderLevel p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;

        }

       public QueryResult<ReOrderLevel> Query(QueryStandard query)
       {
           IQueryable<tblReOrderLevel> reorderQuery;
           if (query.ShowInactive)
               reorderQuery = _ctx.tblReOrderLevel.Where(p => p.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
           else
               reorderQuery = _ctx.tblReOrderLevel.Where(p => p.IM_Status == (int)EntityStatus.Active).AsQueryable();

           var queryResult = new QueryResult<ReOrderLevel>();
           if (!string.IsNullOrEmpty(query.Name))
           {
               reorderQuery = reorderQuery.Where(p => p.tblProduct.Description.ToLower().Contains(query.Name.ToLower())
                                                       || p.tblCostCentre.Distributor_Owner.ToLower().Contains(query.Name.ToLower()));

           }

           reorderQuery = reorderQuery.OrderBy(p => p.tblProduct.Description).ThenBy(p => p.DistributorId);
           queryResult.Count = reorderQuery.Count();

           if (query.Skip.HasValue && query.Take.HasValue)
               reorderQuery = reorderQuery.Skip(query.Skip.Value).Take(query.Take.Value);
           queryResult.Data = reorderQuery.Select(Map).OfType<ReOrderLevel>().ToList();

           return queryResult;
       }

       public ReOrderLevel Map(tblReOrderLevel tblRLevel)
        {
            ReOrderLevel rLevel = new ReOrderLevel(tblRLevel.id) 
            {
                ProductReOrderLevel = tblRLevel.ProductReOrderLevel,
             DistributorId=_costCentreRepository.GetById(tblRLevel.DistributorId),
             ProductId=_productRepository.GetById(tblRLevel.ProductId)
            };
            rLevel._SetDateCreated(tblRLevel.IM_DateCreated);
            rLevel._SetDateLastUpdated(tblRLevel.IM_DateLastUpdated);
            rLevel._SetStatus((EntityStatus)tblRLevel.IM_Status);
            return rLevel;
        }
    }
}
