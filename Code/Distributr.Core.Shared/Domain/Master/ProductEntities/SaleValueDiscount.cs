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
    public class SaleValueDiscount : MasterEntity
    {
        public SaleValueDiscount() : base(default(Guid)) { }

        public SaleValueDiscount(Guid id) : base(id)
        {
            DiscountItems = new List<SaleValueDiscountItem>();
        }
        public SaleValueDiscount(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
            
        }
        public SaleValueDiscount(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive, List<SaleValueDiscountItem> saleValueDiscountItems)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
            DiscountItems = saleValueDiscountItems;
        }
    #if __MOBILE__
        [ForeignKey(typeof(ProductPricingTier))]
        public Guid TierMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public ProductPricingTier Tier { get; set; }
        public decimal CurrentRate { get { return LatestDiscountItem() != null ? LatestDiscountItem().DiscountValue : 0; } }
        public decimal CurrentSaleValue { get { return LatestDiscountItem() != null ? LatestDiscountItem().DiscountThreshold : 0; } }
        public DateTime CurrentEffectiveDate { get { return LatestDiscountItem() != null ? LatestDiscountItem().EffectiveDate : DateTime.Parse("01-jan-1900"); } }
        public DateTime CurrentEndDate { get { return LatestDiscountItem() != null ? LatestDiscountItem().EndDate : DateTime.Parse("01-jan-1900"); } }
        public decimal GetPercentageDiscount(decimal saleValue)
        {
            if (LatestDiscountItem().DiscountThreshold > saleValue)
                return 0;
            return LatestDiscountItem().DiscountValue;
        }
        public decimal GetDiscount(decimal saleValue)
        {
            return saleValue*GetPercentageDiscount(saleValue);
        }
        public SaleValueDiscountItem LatestDiscountItem()
        {
            var latest = DiscountItems
                .FirstOrDefault(n => n._Status == EntityStatus.Active && n.EffectiveDate.Date<=DateTime.Now.Date && n.EndDate.Date>=DateTime.Now.Date);

            return latest;
        }

    #if __MOBILE__
        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public List<SaleValueDiscountItem> DiscountItems { get; set; }

#if !SILVERLIGHT
   [Serializable]
#endif
    #if __MOBILE__
   [Table("SaleValueDiscountItems")]
    #endif
        public class SaleValueDiscountItem : MasterEntity
        {
            public SaleValueDiscountItem() : base(default(Guid)) { }

            public SaleValueDiscountItem(Guid id)
                : base(id)
            {

            }
            public SaleValueDiscountItem(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
                : base(id, dateCreated, dateLastUpdated, isActive)
            {

            }
            public decimal DiscountThreshold { get; set; }
            public decimal DiscountValue { get; set; }
            public DateTime EffectiveDate { get; set; }
            public Guid LineItemId { get; set; }
            public EntityStatus IsActive { get; set; }
            public DateTime EndDate { get; set; }

            #if __MOBILE__
            [ForeignKey(typeof(SaleValueDiscount))]            
            public Guid SaleValueDiscountMasterId { get; set; }
            #endif

        }
    }
}
