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
#if __MOBILE__
    [Table("Pricing")]
#endif
    // creating a base class
    public class ProductPricing : MasterEntity
    {
       public ProductPricing() : base(default(Guid)) { }

        public ProductPricing(Guid id)
            : base(id)
        {
            ProductPricingItems = new List<ProductPricingItem>();

        }
        public ProductPricing(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive){}

        public ProductPricing(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive, List<ProductPricingItem> productPricingItems)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
            ProductPricingItems = productPricingItems;
        }


    #if __MOBILE__       
        [ForeignKey(typeof(Product))]
        public Guid ProductMasterId { get; set; }
       [Ignore]
    #endif
        public ProductRef ProductRef { get; set; }


    #if __MOBILE__
        [ForeignKey(typeof(ProductPricingTier))]
        public Guid ProductPricingTierMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public ProductPricingTier Tier { get; set; }

    #if __MOBILE__
        [Column("ExFactoryRate")]
        public virtual decimal CurrentExFactory { get { return LatestProductPricingItem().ExFactoryRate; } set{} }
    #else 
       public virtual decimal CurrentExFactory { get { return LatestProductPricingItem().ExFactoryRate; } }
    #endif


    #if __MOBILE__
        [Column("SellingPrice")]
        public virtual decimal CurrentSellingPrice { get { return LatestProductPricingItem().SellingPrice; } set{} }
    #else 
       public decimal CurrentSellingPrice { get { return LatestProductPricingItem().SellingPrice; } }
    #endif


    #if __MOBILE__
        [Column("EffectiveDate")]
        public DateTime CurrentEffectiveDate { get { return LatestProductPricingItem().EffectiveDate; } set{} }
    #else
       public DateTime CurrentEffectiveDate { get { return LatestProductPricingItem().EffectiveDate; } }
    #endif


    #if __MOBILE__
        public Guid LineItemId { get; set; }
    #endif

        private ProductPricingItem LatestProductPricingItem()
        {
            var items = ProductPricingItems.Where(n => n.EffectiveDate < DateTime.Now)
                .OrderByDescending(n => n._DateCreated)
                .ThenByDescending(n => n.EffectiveDate);
            return items.ToList().First();
        }

       #if __MOBILE__
         [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        #endif
        public List<ProductPricingItem> ProductPricingItems { get; set; }

#if !SILVERLIGHT
   [Serializable]
#endif
    #if __MOBILE__
       [Table("PricingItems")]
    #endif
        public class ProductPricingItem : MasterEntity
        {
    
            public ProductPricingItem() : base(default(Guid)) { }

            public ProductPricingItem(Guid id)
                : base(id)
            {

            }
            public ProductPricingItem(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
                : base(id, dateCreated, dateLastUpdated, isActive)

            {

            }
            public decimal ExFactoryRate { get; set; }
            public DateTime EffectiveDate { get; set; }
            public decimal SellingPrice { get; set; }

        #if __MOBILE__
            [ForeignKey(typeof(ProductPricing))]
            public Guid ProductPricingMasterId { get; set; }
        #endif
        
        }

    }
}
