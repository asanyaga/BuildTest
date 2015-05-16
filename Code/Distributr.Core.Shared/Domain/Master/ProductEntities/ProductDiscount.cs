using System;
using System.Collections.Generic;
using System.Linq;
#if __MOBILE__
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
#endif

namespace Distributr.Core.Domain.Master.ProductEntities
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class ProductDiscount : MasterEntity
    {
        public ProductDiscount() : base(default(Guid)) { }

        public ProductDiscount(Guid id)
            : base(id)
        {
            DiscountItems = new List<ProductDiscountItem>();
        }

        public ProductDiscount(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
        }

        public ProductDiscount(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive,
                               List<ProductDiscountItem> productDiscountItems)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
            DiscountItems = productDiscountItems;
        }

        #if __MOBILE__
        [ForeignKey(typeof(Product))]
        public Guid ProductMasterId { get; set; }

        [Ignore]
        #endif
        public ProductRef ProductRef { get; set; }


    #if __MOBILE__
        [ForeignKey(typeof(ProductPricingTier))]
        public Guid TierMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public ProductPricingTier Tier { get; set; }
        public decimal CurrentDiscountRate(decimal quantity)
        {
            return LatestProductDiscountItem(quantity) != null ? LatestProductDiscountItem(quantity).DiscountRate : 0; 
        }
        //public decimal CurrentDiscountRate(int quantity)
        //{
        //    get { return LatestProductDiscountItem() != null ? LatestProductDiscountItem().DiscountRate : 0; }
        //}

        public DateTime CurrentEffectiveDate(decimal quantity)
        {

            return LatestProductDiscountItem(quantity) != null ? LatestProductDiscountItem(quantity).EffectiveDate.Date
                           : DateTime.Parse("01-jan-1900");
            
        }

        //public DateTime CurrentEffectiveDate
        //{
        //    get
        //    {
        //        return LatestProductDiscountItem() != null? LatestProductDiscountItem().EffectiveDate.Date
        //                   : DateTime.Parse("01-jan-1900");
        //    }
        //}

        public DateTime CurrentEndDate(decimal quantity)
        {
           
                return LatestProductDiscountItem(quantity) != null ? LatestProductDiscountItem(quantity).EndDate.Date : DateTime.Parse("01-jan-1900");
            
        }

        //public DateTime CurrentEndDate
        //{
        //    get
        //    {
        //        return LatestProductDiscountItem() != null? LatestProductDiscountItem().EndDate.Date: DateTime.Parse("01-jan-1900");
        //    }
        //}
        private ProductDiscountItem LatestProductDiscountItem(decimal quantity)
        {

            var data = DiscountItems.Where(n=> n._Status == EntityStatus.Active && n.EffectiveDate.Date <= DateTime.Now && n.EndDate.Date >= DateTime.Now)
                .OrderByDescending(s => s.Quantity)
                .ThenByDescending(s=>s.EffectiveDate).ToList();
           var latest=data.FirstOrDefault(n => n.Quantity <= quantity );

            return latest;
        }
        //private ProductDiscountItem LatestProductDiscountItem()
        //{
        //    var latest = DiscountItems
        //        .FirstOrDefault(n => n._Status == EntityStatus.Active && n.EffectiveDate.Date<=DateTime.Now && n.EndDate.Date>=DateTime.Now);

        //    return latest;
        //}
    #if __MOBILE__
         [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public List<ProductDiscountItem> DiscountItems { get; set; }

#if !SILVERLIGHT
        [Serializable]
#endif
        public class ProductDiscountItem : MasterEntity
        {
            public ProductDiscountItem() : base(default(Guid)) { }
            public ProductDiscountItem(Guid id)
                : base(id)
            {
            }

            public ProductDiscountItem(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
                : base(id, dateCreated, dateLastUpdated, isActive)
            {
            }

            public decimal DiscountRate { get; set; }
            public DateTime EffectiveDate { get; set; }
        #if __MOBILE__
            [ForeignKey(typeof(ProductDiscount))]
        #endif
            public Guid LineItemId { get; set; }
            public EntityStatus IsActive { get; set; }
            public DateTime EndDate { get; set; }

            public decimal Quantity { get; set; }
            public bool IsByQuantity { get; set; }
        }
    }


}
