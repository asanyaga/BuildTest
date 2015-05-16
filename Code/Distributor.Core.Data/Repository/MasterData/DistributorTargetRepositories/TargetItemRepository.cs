using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.DistributorTargetEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.DistributorTargetRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.DistributorTargetRepositories
{
   internal class TargetItemRepository:RepositoryMasterBase<TargetItem>, ITargetItemRepository
    {
        CokeDataContext _ctx;
        private IProductRepository _productRepository;
        ITargetRepository _targetRepository;
        ICacheProvider _cacheProvider;

        public TargetItemRepository(CokeDataContext ctx, IProductRepository productRepository, ITargetRepository targetRepository, ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _productRepository = productRepository;
            _targetRepository = targetRepository;
            _cacheProvider = cacheProvider;
        }



        public Guid Save(TargetItem entity, bool? isSync = null)
        {
            _log.DebugFormat("Saving/Updating Co");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            DateTime dt = DateTime.Now;
            if (!vri.IsValid)
            {
                throw new DomainValidationException(vri, "Target item is invalid");
            }
            tblTargetItem tblTargetitem = _ctx.tblTargetItem.FirstOrDefault(n => n.Id == entity.Id);
            if (tblTargetitem == null)
            {
                tblTargetitem = new tblTargetItem();
                tblTargetitem.IM_DateCreated = dt;
                tblTargetitem.IM_Status = (int)EntityStatus.Active;//true;
                tblTargetitem.Id = entity.Id;
                _ctx.tblTargetItem.AddObject(tblTargetitem);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tblTargetitem.IM_Status != (int)entityStatus)
                tblTargetitem.IM_Status = (int)entity._Status;

            tblTargetitem.TargetId = _targetRepository.GetById(entity.Target.Id).Id;
            tblTargetitem.ProductId = _productRepository.GetById(entity.Product.ProductId).Id;
            tblTargetitem.Quantity = entity.Quantity;

            tblTargetitem.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblTargetItem.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tblTargetitem.Id));
            return tblTargetitem.Id;
        }

        public void SetInactive(TargetItem entity)
        {
            tblTargetItem tblTargetitem = _ctx.tblTargetItem.FirstOrDefault(n => n.Id == entity.Id);
            if (tblTargetitem != null)
            {
                tblTargetitem.IM_Status = (int)EntityStatus.Inactive;// false;
                tblTargetitem.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblTargetItem.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblTargetitem.Id));
            }
        }

       public void SetActive(TargetItem entity)
       {
           _log.DebugFormat("Activating target item");
           ValidationResultInfo vri = Validate(entity);
           DateTime dt = DateTime.Now;
           if (!vri.IsValid)
           {
               throw new DomainValidationException(vri, "Target item is invalid");
           }
           tblTargetItem tblTargetitem = _ctx.tblTargetItem.FirstOrDefault(n => n.Id == entity.Id);
           if (tblTargetitem != null)
           {
               tblTargetitem.IM_Status = (int)EntityStatus.Active;
               tblTargetitem.IM_DateLastUpdated = DateTime.Now;
               _ctx.SaveChanges();
               _cacheProvider.Put(_cacheListKey, _ctx.tblTargetItem.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
               _cacheProvider.Remove(string.Format(_cacheKey, tblTargetitem.Id));
           }
       }

       public void SetAsDeleted(TargetItem entity)
       {
           _log.DebugFormat("Deleting target item");
           tblTargetItem tblTargetitem = _ctx.tblTargetItem.FirstOrDefault(n => n.Id == entity.Id);
           if (tblTargetitem != null)
           {
               tblTargetitem.IM_Status = (int)EntityStatus.Deleted; 
               tblTargetitem.IM_DateLastUpdated = DateTime.Now;
               _ctx.SaveChanges();
               _cacheProvider.Put(_cacheListKey, _ctx.tblTargetItem.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.Id).ToList());
               _cacheProvider.Remove(string.Format(_cacheKey, tblTargetitem.Id));
           }
       }

       public TargetItem GetById(Guid Id, bool includeDeactivated=false)
        {
            TargetItem entity = (TargetItem)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblTargetItem.FirstOrDefault(s => s.Id == Id);
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
           get { return "TargetItem-{0}"; }
       }

       protected override string _cacheListKey
       {
           get { return "TargetItemList"; }
       }

       public override IEnumerable<TargetItem> GetAll(bool includeDeactivated)
        {
            _log.DebugFormat("Getting All");
            IList<TargetItem> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<TargetItem>(ids.Count);
                foreach (Guid id in ids)
                {
                    TargetItem entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblTargetItem.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (TargetItem p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }

        public ValidationResultInfo Validate(TargetItem itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            return vri;
        }
        protected TargetItem Map(tblTargetItem tblTargetItem)
        {
            TargetItem trgt = new TargetItem(tblTargetItem.Id)
                                  {

                                      Product = new ProductRef { ProductId = tblTargetItem.ProductId },
                                      Quantity = tblTargetItem.Quantity,
                                      Target = _targetRepository.GetById(tblTargetItem.TargetId)

                                  };
            trgt._SetDateCreated(tblTargetItem.IM_DateCreated);
            trgt._SetDateLastUpdated(tblTargetItem.IM_DateLastUpdated);
            trgt._SetStatus((EntityStatus)tblTargetItem.IM_Status);
            return trgt;
        }
    }
}
