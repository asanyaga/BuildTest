using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Repository.Master;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.MappingExtensions;
using Distributr.Core.Factory.Master;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository;
using Distributr.Core.Utility;
using Distributr.Core.Data.Utility.Caching;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Data.Utility;
using Distributr.Core.Utility.Caching;
using Distributr.Core.Utility.MasterData;
using Distributr.Core.Utility.Validation;


namespace Distributr.Core.Data.Repository.MasterData.ProductRepositories
{
    internal class ProductPricingRepository : RepositoryMasterBase<ProductPricing>,IProductPricingRepository
    {
     //calling the coke data context
       CokeDataContext _ctx;
       ICacheProvider _cacheProvider;
       public ProductPricingRepository(CokeDataContext ctx,  ICacheProvider cacheProvider)
       {
           _ctx = ctx;
           _cacheProvider = cacheProvider;           
       }

       public Guid Save(ProductPricing entity, bool? isSync = null)
       {
           var vri = new ValidationResultInfo();
           if (isSync == null || !isSync.Value)
           {
               vri = Validate(entity);
           }
           DateTime dt = DateTime.Now;
           if (!vri.IsValid)
           {
               _log.Debug(CoreResourceHelper.GetText("hq.pricing.validation.error"));
               throw new DomainValidationException(vri, CoreResourceHelper.GetText("hq.ptier.validation.error"));
           }
           tblPricing pricing = _ctx.tblPricing.FirstOrDefault(n => n.id == entity.Id);
           if (pricing == null)
           {

               pricing = new tblPricing();
               pricing.IM_DateCreated = dt;
               pricing.IM_Status = (int)EntityStatus.Active;//true;
               pricing.id = entity.Id;
               _ctx.tblPricing.AddObject(pricing);
           }
           var entityStatus = (entity._Status == EntityStatus.New) ? EntityStatus.Active : entity._Status;
           if (pricing.IM_Status != (int)entityStatus)
               pricing.IM_Status = (int)entity._Status;
             
           foreach (ProductPricing.ProductPricingItem pi in entity.ProductPricingItems
               .Where(p => !pricing.tblPricingItem.Select(s => s.EffecitiveDate.ToString("dd-MMM-yyyy hh:mm:ss ")).Contains(p.EffectiveDate.ToString("dd-MMM-yyyy hh:mm:ss ")))
               )
           {
               tblPricingItem pritem = new tblPricingItem();
               pritem.id = pi.Id != Guid.Empty ? pi.Id : Guid.NewGuid();
               pritem.Exfactory = pi.ExFactoryRate;
               pritem.SellingPrice = pi.SellingPrice;
               pritem.EffecitiveDate = pi.EffectiveDate;
               pritem.IM_DateCreated = dt;
               pritem.IM_DateLastUpdated = dt;
               pritem.IM_Status = (int)EntityStatus.Active;//true;
               pricing.tblPricingItem.Add(pritem);
           }
          

           pricing.ProductRef = entity.ProductRef.ProductId;                     
           pricing.Tier = entity.Tier.Id;          
           pricing.IM_DateLastUpdated = dt;
           _ctx.SaveChanges();
           _cacheProvider.Put(_cacheListKey, _ctx.tblPricing.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
           _cacheProvider.Remove(string.Format(_cacheKey, pricing.id));
           return pricing.id;
       }
       public void SetInactive(ProductPricing entity)
       {
           tblPricing pricing = _ctx.tblPricing.FirstOrDefault(p => p.id == entity.Id);
           if (pricing != null)
           {
               pricing.IM_Status = (int)EntityStatus.Inactive;//false;
               pricing.IM_DateLastUpdated = DateTime.Now;
               _ctx.SaveChanges();
               _cacheProvider.Put(_cacheListKey, _ctx.tblPricing.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
               _cacheProvider.Remove(string.Format(_cacheKey, pricing.id));
           }
       }

        public void SetActive(ProductPricing entity)
        {
            tblPricing tblPricing = _ctx.tblPricing.FirstOrDefault(n => n.id == entity.Id);
            if (tblPricing != null)
            {
                tblPricing.IM_Status = (int) EntityStatus.Active;
                tblPricing.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblPricing.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(n => n.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblPricing.id));
            }
        }

        public void SetAsDeleted(ProductPricing entity)
        {
            tblPricing tblPricing = _ctx.tblPricing.FirstOrDefault(p => p.id == entity.Id);
            if (tblPricing != null)
            {
                tblPricing.IM_Status = (int)EntityStatus.Deleted;//false;
                tblPricing.IM_DateLastUpdated = DateTime.Now;
                _ctx.SaveChanges();
                _cacheProvider.Put(_cacheListKey, _ctx.tblPricing.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
                _cacheProvider.Remove(string.Format(_cacheKey, tblPricing.id));
            }
        }

        public ProductPricing GetById(Guid Id, bool includeDeactivated = false)
       {
           ProductPricing entity = (ProductPricing)_cacheProvider.Get(string.Format(_cacheKey, Id));
           if (entity == null)
           {
               var tbl = _ctx.tblPricing.FirstOrDefault(s => s.id == Id);
               if (tbl != null)
               {
                   entity = tbl.Map();
                   _cacheProvider.Put(string.Format(_cacheKey, entity.Id), entity);
               }

           }
           return entity;
       }

        protected override string _cacheKey
        {
            get { return "ProductPricing-{0}"; }
        }

        protected override string _cacheListKey
        {
            get { return "ProductPricingList"; }
        }

        public override IEnumerable<ProductPricing> GetAll(bool includeDeactivated = false)
       {
           IList<ProductPricing> entities = null;
           IList<Guid> ids = (IList<Guid>)_cacheProvider.Get(_cacheListKey);
           if (ids != null)
           {
               entities = new List<ProductPricing>(ids.Count);
               foreach (Guid id in ids)
               {
                   ProductPricing entity = GetById(id);
                   if (entity != null)
                       entities.Add(entity);
               }
           }
           else
           {
               entities = _ctx.tblPricing.Where(n => n.IM_Status != (int)EntityStatus.Deleted).ToList().Select(s => s.Map()).ToList();
               if (entities != null && entities.Count > 0)
               {
                   ids = entities.Select(s => s.Id).ToList(); //new List<int>(persons.Count);
                   _cacheProvider.Put(_cacheListKey, ids);
                   foreach (ProductPricing p in entities)
                   {
                       _cacheProvider.Put(string.Format(_cacheKey, p.Id), p);
                   }

               }
           }

           if (!includeDeactivated)
               entities = entities.Where(n => n._Status != EntityStatus.Inactive).ToList();
           return entities;
       }

        public ProductPricing GetByProductAndTierId(Guid productId, Guid tierId)
        {
            var pricing = _ctx.tblPricing.FirstOrDefault(n => n.Tier == tierId && n.ProductRef == productId);
            return pricing != null ? pricing.Map() : null;
        }

        public QueryResult<ProductPricing> Query(QueryStandard q)
        {
            IQueryable<tblPricing> productPricingQuery;

            if (q.ShowInactive)
                productPricingQuery =
                    _ctx.tblPricing.Where(n => n.IM_Status != (int)EntityStatus.Deleted).AsQueryable();
            else
                productPricingQuery =
                    _ctx.tblPricing.Where(n => n.IM_Status == (int)EntityStatus.Active).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q.Name))
                productPricingQuery =
                    productPricingQuery.Where(n => n.tblProduct.Description.ToLower().Contains(q.Name.ToLower()));

            if (q.SupplierId.HasValue)
                productPricingQuery =
                    productPricingQuery.Where(l => l.tblProduct.tblProductBrand.SupplierId == q.SupplierId);

            var queryResult = new QueryResult<ProductPricing>();
            queryResult.Count = productPricingQuery.Count();
            productPricingQuery = productPricingQuery.OrderBy(m => m.Tier).ThenBy(l => l.tblProduct.Description);

            if (q.Skip.HasValue && q.Take.HasValue)
                productPricingQuery = productPricingQuery.Skip(q.Skip.Value).Take(q.Take.Value);

            var result = productPricingQuery.ToList();

            queryResult.Data = result.Select(s => s.Map()).ToList();

            q.ShowInactive = false;

            return queryResult;
        }

        public ValidationResultInfo Validate(ProductPricing itemToValidate)
       {
           ValidationResultInfo vri = itemToValidate.BasicValidation();
           if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
               return vri;
           if (itemToValidate.Id == Guid.Empty)
               vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));

           var exp =
               _ctx.tblPricing.Where(
                   s =>
                   s.id != itemToValidate.Id && s.ProductRef== itemToValidate.ProductRef.ProductId &&
                   s.Tier == itemToValidate.Tier.Id);

          var isDuplicate =
               exp.SelectMany(n =>n.tblPricingItem).ToList().Count(x =>
                       x.EffecitiveDate.ToString("dd-MMM-yyyy") ==
                       itemToValidate.CurrentEffectiveDate.ToString("dd-MMM-yyyy"))>1;
          if (isDuplicate)
               vri.Results.Add(new ValidationResult(CoreResourceHelper.GetText("hq.pricing.validation.dupprod")));
          
           
           return vri;
       }


        public void AddProductPricing(Guid productPricingId, decimal exFactoryPrice, decimal sellingPrice, DateTime effectiveDate)
        {
           
           ProductPricing price = GetById(productPricingId);

           if (price!=null)
           {
               tblPricing tblPricing = _ctx.tblPricing.FirstOrDefault(n => n.id == productPricingId);
               if (price.ProductPricingItems.Any(p => p.EffectiveDate.ToString("dd/MM/yyyy") == effectiveDate.ToString("dd/MM/yyyy")))
               {
                   throw new DomainValidationException(this.BasicValidation(), "Only one effective price per day");
               }
               tblPricingItem pritem = new tblPricingItem();
               pritem.id =  Guid.NewGuid();
               pritem.Exfactory = exFactoryPrice;
               pritem.SellingPrice = sellingPrice;
               pritem.EffecitiveDate = effectiveDate;
               pritem.IM_DateCreated = DateTime.Now;
               pritem.IM_DateLastUpdated = DateTime.Now; ;
               pritem.IM_Status = (int)EntityStatus.Active;//true;
               pritem.PricingId = price.Id;
               tblPricing.tblPricingItem.Add(pritem);
               tblPricing.IM_DateLastUpdated = DateTime.Now;
               _ctx.SaveChanges();
               _cacheProvider.Put(_cacheListKey, _ctx.tblPricing.Where(n => n.IM_Status != (int)EntityStatus.Deleted).Select(s => s.id).ToList());
               _cacheProvider.Remove(string.Format(_cacheKey, price.Id));
               //price.ProductPricingItems.Add(new ProductPricing.ProductPricingItem(Guid.NewGuid())
               //{
               //    EffectiveDate = effectiveDate,
               //    ExFactoryRate = exFactoryPrice,
               //    SellingPrice = sellingPrice
               //});

               //Save(price);
           }
           else
           {
               throw new DomainValidationException(this.BasicValidation(), "Product price not set");
           }
          

       }

        
    }
}
