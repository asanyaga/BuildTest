using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Data.EF;
using Distributr.Core.Utility;
using Distributr.Core.Data.Utility.Caching;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.MasterData.ProductRepositories
{
    internal class ProductDiscountRepository : RepositoryMasterBase<ProductDiscount>, IProductDiscountRepository
    {
        CokeDataContext _ctx;
        ICacheProvider _cacheProvider;
        IProductRepository _productRepository;
        IProductPricingTierRepository _productPricingTierRepository;
        public ProductDiscountRepository(CokeDataContext ctx, IProductRepository productRepository, ICacheProvider cacheProvider, IProductPricingTierRepository productPricingTierRepository)
        {
            _ctx = ctx;
            _productRepository = productRepository;
            _cacheProvider = cacheProvider;
            _productPricingTierRepository = productPricingTierRepository;
        }

        public Guid Save(ProductDiscount entity, bool? isSync = null)
        {
            _log.DebugFormat("Saving/Updating Product Discount");
            var vri = new ValidationResultInfo();
            if (isSync == null || !isSync.Value)
            {
                vri = Validate(entity);
            }
            if (!vri.IsValid)
            {
                var discount = entity.DiscountItems
                    .FirstOrDefault(n => n._Status == EntityStatus.New);
                entity.DiscountItems.Remove(discount);
                throw new DomainValidationException(vri, "Failed to validate product discount");
            }
            DateTime dt = DateTime.Now;
            tblDiscounts tblDisc = _ctx.tblDiscounts.FirstOrDefault(n => n.id == entity.Id);
            if (tblDisc == null)
            {
                tblDisc = new tblDiscounts();
                tblDisc.IM_DateCreated = dt;
                tblDisc.IM_Status = (int)EntityStatus.Active;
                tblDisc.id = entity.Id;
                _ctx.tblDiscounts.AddObject(tblDisc);
            }
            var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
            if (tblDisc.IM_Status != (int)entityStatus)
                tblDisc.IM_Status = (int)entity._Status;

            //tblDisc.tblDiscountItem.Where(n => n.DiscountId == entity.Id &&
            //    n.IM_Status != (int) EntityStatus.Deleted).ToList()
            //    .ForEach(n => n.IM_Status = (int) EntityStatus.Deleted);

            foreach (var discountItem in entity.DiscountItems)
            {
                if (discountItem.IsActive == EntityStatus.New)
                {
                    var discItem = _ctx.tblDiscountItem.FirstOrDefault(n => n.id == discountItem.Id);
                    if (discItem == null)
                    {
                        discItem = new tblDiscountItem
                                       {
                                           id = discountItem.Id,
                                           EffectiveDate = discountItem.EffectiveDate,
                                           EndDate = discountItem.EndDate,
                                           DiscountRate = discountItem.DiscountRate,
                                           IM_DateCreated = dt,
                                           IM_DateLastUpdated = dt,
                                           IM_Status = (int) EntityStatus.Active,
                                           IsByQuantity = discountItem.IsByQuantity,
                                           Quantity = discountItem.Quantity,

                                       };
                        tblDisc.tblDiscountItem.Add(discItem);
                    }
                    else
                    {
                        // _ctx.SaveChanges();
                        discItem.IM_Status = (int) EntityStatus.Active;
                    }
                    discItem.IsByQuantity = discountItem.IsByQuantity;
                    discItem.Quantity = discountItem.Quantity;
                }
            }

            tblDisc.IM_DateLastUpdated = dt;
            tblDisc.ProductRef = entity.ProductRef.ProductId;
            tblDisc.TierId = entity.Tier.Id;
            _ctx.SaveChanges();
            _cacheProvider.Put(_cacheListKey, _ctx.tblDiscounts.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
            _cacheProvider.Remove(string.Format(_cacheKey, tblDisc.id));
            return tblDisc.id;
        }

        

        protected override string _cacheKey
        {
            get { return "ProductDiscount-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "ProductDiscountList"; }
        }

        public override IEnumerable<ProductDiscount> GetAll(bool includeDeactivated = false)
        {
            _log.DebugFormat("Get All");
            IList<ProductDiscount> entities = null;
            IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
            if (ids != null)
            {
                entities = new List<ProductDiscount>(ids.Count);
                foreach (Guid id in ids)
                {
                    ProductDiscount entity = GetById(id);
                    if (entity != null)
                        entities.Add(entity);
                }
            }
            else
            {
                entities = _ctx.tblDiscounts.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => Map(s)).ToList();
                if (entities != null && entities.Count > 0)
                {
                    ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                    _cacheProvider.Put(_cacheListKey, ids);
                    foreach (ProductDiscount p in entities)
                    {
                        _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                    }

                }
            }

            if (!includeDeactivated)
                entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
            return entities;
          
        }

        public ProductDiscount Map(tblDiscounts discounts)
        {
            ProductDiscount prdDisc = new ProductDiscount(discounts.id)
            {
                ProductRef = new ProductRef { ProductId = discounts.ProductRef },
                Tier = _productPricingTierRepository.GetById(discounts.TierId)

            };
            prdDisc.DiscountItems = discounts.tblDiscountItem.Where(s => s.IM_Status == (int)EntityStatus.Active)
                .Select(s => new ProductDiscount.ProductDiscountItem(s.id, s.IM_DateCreated, s.IM_DateLastUpdated, (EntityStatus)s.IM_Status)
            {
                LineItemId = s.id,
                DiscountRate = s.DiscountRate,
                EffectiveDate = s.EffectiveDate,
                EndDate =  s.EndDate ?? s.EffectiveDate,
                IsActive =(EntityStatus) s.IM_Status,
                 IsByQuantity = s.IsByQuantity,
                                           Quantity = s.Quantity,
            }).ToList();
            prdDisc._SetDateCreated(discounts.IM_DateCreated);
            prdDisc._SetDateLastUpdated(discounts.IM_DateLastUpdated);
            prdDisc._SetStatus((EntityStatus)discounts.IM_Status);
            return prdDisc;
        }

        public void AddDiscount(Guid discountId, DateTime effectiveDate, decimal discountRate, DateTime endDate, bool isByQuantity, decimal quantity)
        {
            ProductDiscount pd = GetById(discountId);
            if (pd != null)
            {
                pd.DiscountItems.Add(new ProductDiscount.ProductDiscountItem(Guid.NewGuid())
                {
                    EffectiveDate = effectiveDate,
                    DiscountRate = discountRate,
                    EndDate = endDate,
                    _Status = EntityStatus.Active,
                    IsByQuantity = isByQuantity,
                    Quantity = quantity,
                });
                Save(pd);
            }
            else
            {
                throw new DomainValidationException(this.BasicValidation(), "Failed discount not set");
            }
        }

        public ProductDiscount GetProductDiscount(Guid productId, Guid tierId)
        {
            var tblProductDiscount = _ctx.tblDiscounts.FirstOrDefault(k=>k.ProductRef == productId && k.IM_Status == (int)EntityStatus.Active && k.TierId == tierId);
            if (tblProductDiscount != null)
                return Map(tblProductDiscount);
                
            return null; 
        }

        public QueryResult<ProductDiscount> Query(QueryStandard q)
        {
            IQueryable<tblDiscounts> discountquery;
            IQueryable<tblProduct> productQuery;
            productQuery = _ctx.tblProduct.AsQueryable();
            discountquery = _ctx.tblDiscounts.AsQueryable();

            var query = from d in discountquery
                        join e in productQuery on d.ProductRef equals e.id
                        select new { ProductDiscount=d, Product=e};
            if (q.ShowInactive)
                query = query.Where(l => l.ProductDiscount.IM_Status == (int)EntityStatus.Deleted);
            else
                query = query.Where(l => l.ProductDiscount.IM_Status == (int)EntityStatus.Active);

            if (!string.IsNullOrWhiteSpace(q.Name))
                query = query.Where(l => l.Product.Description.ToLower().Contains(q.Name.ToLower()));

            if (q.SupplierId.HasValue)
                query = query.Where(k => k.Product.tblProductBrand.SupplierId == q.SupplierId.Value);

            var queryResult = new QueryResult<ProductDiscount>();
            queryResult.Count = query.Count();

            query = query.OrderBy(k => k.ProductDiscount.id).ThenBy(l => l.Product.Description);

            if (q.Skip.HasValue && q.Take.HasValue)
                query = query.Skip(q.Skip.Value).Take(q.Take.Value);

            var result = query.ToList();
            queryResult.Data = result.Select(l => Map(l.ProductDiscount)).ToList();

            q.ShowInactive = false;
            return queryResult;
        }

        public void SetInactive(ProductDiscount entity)
        {
            tblDiscounts tblDisc = _ctx.tblDiscounts.FirstOrDefault(n => n.id == entity.Id);
            if (tblDisc != null)
            {
                tblDisc.IM_Status = (int)EntityStatus.Inactive;// false;
                tblDisc.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblDiscounts.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblDisc.id));
            }
        }

        public void SetActive(ProductDiscount entity)
        {
            tblDiscounts tblDisc = _ctx.tblDiscounts.FirstOrDefault(n => n.id == entity.Id);
            if (tblDisc != null)
            {
                tblDisc.IM_Status = (int)EntityStatus.Active;// false;
                tblDisc.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblDiscounts.Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblDisc.id));
            }
        }

        public void SetAsDeleted(ProductDiscount entity)
        {
            tblDiscounts tblDisc = _ctx.tblDiscounts.FirstOrDefault(n => n.id == entity.Id);
            if (tblDisc != null)
            {
                tblDisc.IM_Status = (int)EntityStatus.Deleted;// false;
                tblDisc.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblDiscounts.Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblDisc.id));
            }
        }

        public ValidationResultInfo Validate(ProductDiscount itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));

            ProductDiscount.ProductDiscountItem discount = null;
            discount = itemToValidate.DiscountItems
                .FirstOrDefault(n => n._Status == EntityStatus.New);
            if (discount != null)
            {
                if (discount.EndDate.Date < discount.EffectiveDate.Date)
                    vri.Results.Add(new ValidationResult("Invalid date range - End date cannot be lower than start date"));
            }
            return vri;
        }

        public ProductDiscount GetById(Guid Id, bool includeDeactivated = false)
        {
            ProductDiscount entity = (ProductDiscount)_cacheProvider.Get(string.Format(_cacheKey, Id));
            if (entity == null)
            {
                var tbl = _ctx.tblDiscounts.FirstOrDefault(s => s.id == Id);
                if (tbl != null)
                {
                    entity = Map(tbl);
                    _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
                }

            }
            return entity;
        }

        public void DeactivateLineItem(Guid lineItemId)
        {
            var discountlineItem = _ctx.tblDiscountItem.FirstOrDefault(p => p.id == lineItemId);
            if (discountlineItem != null)
            {
                discountlineItem.tblDiscounts.IM_DateLastUpdated = DateTime.Now;
                discountlineItem.IM_Status = (int)EntityStatus.Inactive;// false;
                discountlineItem.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblDiscounts.Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, discountlineItem.DiscountId));
            }
        }
    }
}
