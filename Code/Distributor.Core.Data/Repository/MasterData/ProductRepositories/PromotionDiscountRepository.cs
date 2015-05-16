using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Data.Utility.Caching;
using Distributr.Core.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.ProductRepositories
{
    internal class PromotionDiscountRepository : RepositoryMasterBase<PromotionDiscount>, IPromotionDiscountRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        public PromotionDiscountRepository(CokeDataContext ctx,ICacheProvider cacheProvider)
        {
            _ctx = ctx;
            _cacheProvider = cacheProvider;
        }

        public Guid Save(PromotionDiscount entity, bool? isSync = null)
        {
            _log.InfoFormat("Saving/Upating Promotion discount");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                var discount = entity.PromotionDiscountItems
                    .FirstOrDefault(n => n._Status == EntityStatus.New);
                entity.PromotionDiscountItems.Remove(discount);
                throw new DomainValidationException(vri, "Failed to validate promotion discount");
            }
            DateTime dt = DateTime.Now;
            tblPromotionDiscount tblPromo = _ctx.tblPromotionDiscount.FirstOrDefault(n => n.id == entity.Id);
            if (tblPromo == null)
            {
                tblPromo = new tblPromotionDiscount();
                tblPromo.IM_DateCreated = dt;
                tblPromo.IM_Status = (int)EntityStatus.Active;
                tblPromo.id = entity.Id;
                _ctx.tblPromotionDiscount.AddObject(tblPromo);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tblPromo.IM_Status != (int)entityStatus)
                tblPromo.IM_Status = (int)entity._Status;

                tblPromo.tblPromotionDiscountItem.Where(n => n.PromotionDiscountId == entity.Id &&
                    n.IM_Status != (int) EntityStatus.Deleted).ToList()
                    .ForEach(n => n.IM_Status = (int) EntityStatus.Deleted);

            foreach (var discountItem in entity.PromotionDiscountItems)
            {
                if (discountItem._Status == EntityStatus.New)
                {
                   var tblPromoI = _ctx.tblPromotionDiscountItem.FirstOrDefault(n => n.id == discountItem.Id);
                   if (tblPromoI == null)
                   {
                       tblPromoI = new tblPromotionDiscountItem
                                       {
                                           id = discountItem.Id,
                                           ParentProductQuantity = discountItem.ParentProductQuantity,
                                           FreeOfChargeQuantity = discountItem.FreeOfChargeQuantity,
                                           EffectiveDate = discountItem.EffectiveDate,
                                           FreeOfChargeProductRef = discountItem.FreeOfChargeProduct == null
                                                                        ? Guid.Empty
                                                                        : discountItem.FreeOfChargeProduct.ProductId,
                                           DiscountRate = discountItem.DiscountRate,
                                           EndDate = discountItem.EndDate,
                                           IM_DateCreated = dt,
                                           IM_DateLastUpdated = dt,
                                           IM_Status = (int) EntityStatus.Active
                                       };
                       tblPromo.tblPromotionDiscountItem.Add(tblPromoI);
                   }
                   else
                   {
                       tblPromoI.IM_Status = (int) EntityStatus.Active;
                   }
                }
            }
            tblPromo.ProductRef = entity.ProductRef.ProductId;
            tblPromo.IM_DateLastUpdated = dt;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblPromotionDiscount.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tblPromo.id));
            return tblPromo.id;
        }

        private ValidationResultInfo Validate(PromotionDiscount.PromotionDiscountItem itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;

            if (itemToValidate.EndDate < itemToValidate.EffectiveDate)
                vri.Results.Add(new ValidationResult("Invalid date range - End date cannot be lower than start date"));

            return vri;
        }

        public void SetInactive(PromotionDiscount entity)
        {
            tblPromotionDiscount tblFoc = _ctx.tblPromotionDiscount.FirstOrDefault(n=>n.id==entity.Id);
            if (tblFoc != null)
            {
                tblFoc.IM_DateLastUpdated = DateTime.Now;
                tblFoc.IM_Status = (int)EntityStatus.Inactive;// false;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblPromotionDiscount.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblFoc.id));

            }
           
        }

        public void SetActive(PromotionDiscount entity)
        {
            tblPromotionDiscount tblFoc = _ctx.tblPromotionDiscount.FirstOrDefault(n => n.id == entity.Id);
            if (tblFoc != null)
            {
                tblFoc.IM_DateLastUpdated = DateTime.Now;
                tblFoc.IM_Status = (int)EntityStatus.Active;// false;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblPromotionDiscount.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblFoc.id));
            }
        }

        public void SetAsDeleted(PromotionDiscount entity)
        {
            tblPromotionDiscount tblPromotionDiscount = _ctx.tblPromotionDiscount.FirstOrDefault(l => l.id == entity.Id);
            if(tblPromotionDiscount != null)
            {
                tblPromotionDiscount.IM_Status = (int)EntityStatus.Deleted;// false;
                tblPromotionDiscount.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblPromotionDiscount.Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblPromotionDiscount.id));
            }
        }

        public PromotionDiscount GetById(Guid Id, bool includeDeactivated = false)
        {
            PromotionDiscount entity = (PromotionDiscount)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblPromotionDiscount.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public ValidationResultInfo Validate(PromotionDiscount itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            PromotionDiscount.PromotionDiscountItem discount = null;
            discount = itemToValidate.PromotionDiscountItems
                    .FirstOrDefault(n => n._Status == EntityStatus.New);
            if (discount != null)
            {
                if (discount.EndDate.Date < discount.EffectiveDate.Date)
                    vri.Results.Add(new ValidationResult("Invalid date range - End date cannot be lower than start date"));
            }
            return vri;
        }

        public void AddFreeOfChargeDiscount(Guid focId, int parentProductQuantity, Guid? freeOfChargeProduct, int? freeOfChargeQuantity, DateTime effectiveDate, decimal DiscountRate, DateTime endDate)
        {
            PromotionDiscount foc = GetById(focId);
            if (foc != null)
            {
                foc.PromotionDiscountItems.Add(new PromotionDiscount.PromotionDiscountItem(Guid.NewGuid())
                    {
                        EffectiveDate = effectiveDate,
                        FreeOfChargeProduct = freeOfChargeProduct == null ? null : new ProductRef { ProductId = freeOfChargeProduct.Value },
                        FreeOfChargeQuantity = freeOfChargeQuantity.Value,
                        ParentProductQuantity = parentProductQuantity,
                        DiscountRate = DiscountRate,
                        EndDate = endDate,
                        _Status = EntityStatus.Active
                    });
                Save(foc);
            }
            else
            {
                throw new DomainValidationException(this.BasicValidation(),"Discount not set");
            }
        }

        public PromotionDiscount GetByProductId(Guid productMasterId)
        {
            var all = GetAll();
            return all.Where(p => p.ProductRef.ProductId == productMasterId).FirstOrDefault();
        }

        public PromotionDiscount GetByProductAndQuantity(Guid productMasterId, int quantity)
        {
            var tblPromotionDiscount = _ctx.tblPromotionDiscount
                .FirstOrDefault(p => p.ProductRef == productMasterId
                    && p.tblPromotionDiscountItem.Select(pi => pi.ParentProductQuantity)
                    .Contains(quantity) && p.IM_Status == (int)EntityStatus.Active);
            if (tblPromotionDiscount != null)
                return Map(tblPromotionDiscount);
            return null;
        }

        public PromotionDiscount GetCurrentDiscount(Guid productMasterId)
        {
            return
                GetAll().FirstOrDefault( 
                    s => s.ProductRef !=null && s.ProductRef.ProductId==productMasterId  &&
                    s.CurrentEffectiveDate.Date <= DateTime.Now.Date && s.CurrentEndDate.Date >= DateTime.Now.Date);
        }

        public QueryResult<PromotionDiscount> Query(QueryStandard query)
        {
            IQueryable<tblPromotionDiscount> pdQuery;

            pdQuery = _ctx.tblPromotionDiscount.Where(l => l.IM_Status == (int)EntityStatus.Active).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
                pdQuery = pdQuery.Where(l => l.tblProduct.Description.ToLower().Contains(query.Name.ToLower()));

            if (query.SupplierId.HasValue)
                pdQuery = pdQuery.Where(j => j.tblProduct.tblProductBrand.SupplierId == query.SupplierId.Value);

            var result = new QueryResult<PromotionDiscount>();
            result.Count = pdQuery.Count();

            pdQuery = pdQuery.OrderBy(l => l.tblProduct.Description); 
            
            if (query.Skip.HasValue && query.Take.HasValue)
                pdQuery = pdQuery.Skip(query.Skip.Value).Take(query.Take.Value);

            var queryResult = pdQuery.ToList();

            result.Data = queryResult.Select(Map).OfType<PromotionDiscount>().ToList();

            query.ShowInactive = false;

            return result;
        }

        protected override string _cacheKey
        {
            get { return "PromotionDiscount-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "PromotionDiscountList"; }
        }

        public override IEnumerable<PromotionDiscount> GetAll(bool includeDeactivated = false)
        {
            _log.InfoFormat("Getting All");
            IList<PromotionDiscount> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<PromotionDiscount>(ids.Count);
                foreach (Guid id in ids)
                {
                    PromotionDiscount entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblPromotionDiscount.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (PromotionDiscount p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
        }
        
        PromotionDiscount Map(tblPromotionDiscount tblFoc)
        {
            PromotionDiscount foc = new PromotionDiscount(tblFoc.id)
            {
                ProductRef = new ProductRef { ProductId=tblFoc.ProductRef }

            };
            foc.PromotionDiscountItems =
                tblFoc.tblPromotionDiscountItem.Where(s => s.IM_Status == (int)EntityStatus.Active).Select(
                    s => new PromotionDiscount.PromotionDiscountItem(s.id, s.IM_DateCreated, s.IM_DateLastUpdated, (EntityStatus)s.IM_Status)
                             {
                                 ParentProductQuantity = s.ParentProductQuantity,

                                 FreeOfChargeQuantity = s.DiscountRate == null ? 0 : s.FreeOfChargeQuantity.Value,
                                 FreeOfChargeProduct =
                                     s.FreeOfChargeProductRef == null
                                         ? null
                                        : new ProductRef {ProductId = s.FreeOfChargeProductRef.Value},
                                 EffectiveDate = s.EffectiveDate,
                                 EndDate = s.EndDate ?? s.EffectiveDate,
                                 DiscountRate = s.DiscountRate == null ? 0 : s.DiscountRate.Value,
                                 LineItemId = s.id,
                                 IsActive = (EntityStatus)s.IM_Status
                             }).ToList();
            foc._SetStatus((EntityStatus)tblFoc.IM_Status);
            foc._SetDateLastUpdated(tblFoc.IM_DateLastUpdated);
            foc._SetDateCreated(tblFoc.IM_DateCreated);

            return foc;
        }

        public void DeactivateLineItem(Guid lineItemId)
        {
            var discountlineItem = _ctx.tblPromotionDiscountItem.FirstOrDefault(p => p.id == lineItemId);
            if (discountlineItem != null)
            {
                discountlineItem.IM_Status = (int)EntityStatus.Inactive;// false;
                discountlineItem.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblPromotionDiscount.Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, discountlineItem.PromotionDiscountId));
            }
        }
    }
}
