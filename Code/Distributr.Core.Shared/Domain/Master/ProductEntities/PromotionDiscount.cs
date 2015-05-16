using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Master.ProductEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public class PromotionDiscount:MasterEntity
    {
        public PromotionDiscount(Guid id) : base(id)
        {
            PromotionDiscountItems = new List<PromotionDiscountItem>();
        }
        public PromotionDiscount(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {}

        public PromotionDiscount(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive, List<PromotionDiscountItem> promotionDiscountItems)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
            PromotionDiscountItems = promotionDiscountItems;
        }
        public ProductRef ProductRef { get; set; }
        public ProductPricingTier Tier { get; set; }

       public DateTime CurrentEffectiveDate
       {
           get
           {
               return LatestDiscountItem() != null
                          ? LatestDiscountItem().EffectiveDate
                          : DateTime.Parse("01-jan-1900");
           }
       }

       public DateTime CurrentEndDate
       {
           get
           {
               return LatestDiscountItem() != null
                          ? LatestDiscountItem().EndDate.Date
                          : DateTime.Parse("01-jan-1900");
           }
       }

       public ProductRef CurrentFreeOfChargeProduct { get { return LatestDiscountItem() != null ? LatestDiscountItem().FreeOfChargeProduct : null; } }
        public int CurrentFreeOfChargeQuantity { get { return LatestDiscountItem() !=null ? LatestDiscountItem().FreeOfChargeQuantity : 0; } }
        public int CurrentParentProductQuantity { get { return  LatestDiscountItem() !=null ? LatestDiscountItem().ParentProductQuantity : 0; } }
        public decimal CurrentDiscountRate { get { return LatestDiscountItem() !=null ? LatestDiscountItem().DiscountRate : 0; } }

        public ProductRef CurrentFreeProduct(decimal qty)
        {
            return LatestDiscountItem().FreeOfChargeProduct;
        }

        
       //cn::
        public PromotionDiscountItem AwardedPromotionDiscountItem(decimal qty)
        {
            var item =
                PromotionDiscountItems.OrderBy(o => o.ParentProductQuantity).FirstOrDefault(
                    n =>
                    n.EffectiveDate.Date <= DateTime.Now.Date && n.EndDate.Date >= DateTime.Now.Date &&
                    n.ParentProductQuantity <= qty);


            return item;
        }

        private PromotionDiscountItem LatestDiscountItem()
        {
            var latest = PromotionDiscountItems
                .FirstOrDefault(n => n._Status == EntityStatus.Active && n.EffectiveDate.Date<=DateTime.Now.Date && n.EndDate.Date>= DateTime.Now.Date);

            return latest;
        }

        public List<PromotionDiscountItem> PromotionDiscountItems { get; set; }

#if !SILVERLIGHT
   [Serializable]
#endif
        public class PromotionDiscountItem : MasterEntity
        {
            public PromotionDiscountItem(Guid id)
                : base(id) { }

            public PromotionDiscountItem(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
                : base(id, dateCreated, dateLastUpdated, isActive) { }

           
            public int ParentProductQuantity { get; set; }
            public ProductRef FreeOfChargeProduct { get; set; }
            public int FreeOfChargeQuantity { get; set; }
            public DateTime EffectiveDate { get; set; }
            public decimal DiscountRate { get; set; }
            public Guid LineItemId { get; set; }
            public EntityStatus IsActive { get; set; }
            public DateTime EndDate { get; set; }
        }
    }
}
