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
   public class CertainValueCertainProductDiscount:MasterEntity
    {
       public CertainValueCertainProductDiscount() : base(default(Guid)) { }

       public CertainValueCertainProductDiscount(Guid id) : base(id)
        {
            CertainValueCertainProductDiscountItems = new List<CertainValueCertainProductDiscountItem>();
        }
       public CertainValueCertainProductDiscount(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {}

       public CertainValueCertainProductDiscount(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive, List<CertainValueCertainProductDiscountItem> certainValueCertainProductDiscount)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
            CertainValueCertainProductDiscountItems = certainValueCertainProductDiscount;
        }

        public decimal InitialValue { get; set; }
        public DateTime CurrentEffectiveDate { get { return LatestDiscountItem()!=null ? LatestDiscountItem().EffectiveDate : DateTime.Parse("01-jan-1900"); } }
        public ProductRef CurrentProduct { get { return LatestDiscountItem()!=null ? LatestDiscountItem().Product : null; } }
        public int CurrentQuantity { get { return LatestDiscountItem()!=null ? LatestDiscountItem().Quantity : 0; } }
        public decimal CurrentValue { get { return LatestDiscountItem()!=null ? LatestDiscountItem().CertainValue : 0; } }
        public DateTime CurrentEndDate { get { return LatestDiscountItem() != null ? LatestDiscountItem().EndDate : DateTime.Parse("01-jan-1900"); } }

        private CertainValueCertainProductDiscountItem LatestDiscountItem()
        {
            var latest = CertainValueCertainProductDiscountItems.FirstOrDefault(n => n._Status == EntityStatus.Active && n.EffectiveDate.Date<= DateTime.Now.Date && n.EndDate.Date>=DateTime.Now.Date);
            return latest;
        }

        public CertainValueCertainProductDiscountItem LatestFreeOfChargeDiscountItem()
        {
            var latest = CertainValueCertainProductDiscountItems.FirstOrDefault(n => n._Status == EntityStatus.Active);
            return latest;
        }

        public decimal CertainValue { get; set; }

    #if __MOBILE__
        [Ignore]
    #endif
        public ProductRef Product { get; set; }
        public int Quantity { get; set; }
        public DateTime EffectiveDate { get; set; }
    #if __MOBILE__
        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public List<CertainValueCertainProductDiscountItem> CertainValueCertainProductDiscountItems { get; set; }
#if !SILVERLIGHT
   [Serializable]
#endif  
       #if __MOBILE__
        [Table("CertainValueCertainProductDiscountItems")]
       #endif
       public class CertainValueCertainProductDiscountItem : MasterEntity
        {
            public CertainValueCertainProductDiscountItem() : base(default(Guid)) { }

            public CertainValueCertainProductDiscountItem(Guid id)
                : base(id) { }

            public CertainValueCertainProductDiscountItem(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
                : base(id, dateCreated, dateLastUpdated, isActive) { }
           
            public decimal CertainValue { get; set; }
        #if __MOBILE__
            [Ignore]
        #endif
            public ProductRef Product { get; set; }

            public int Quantity { get; set; }
            public DateTime EffectiveDate { get; set; }

        #if __MOBILE__
            [ForeignKey(typeof(CertainValueCertainProductDiscount))]
            public Guid CertainValueCertainProductDiscountId {get; set; }
        #endif

            public Guid LineItemId { get; set; }
            public EntityStatus IsActive { get; set; }
            public DateTime EndDate { get; set; }


        }
    }
}
