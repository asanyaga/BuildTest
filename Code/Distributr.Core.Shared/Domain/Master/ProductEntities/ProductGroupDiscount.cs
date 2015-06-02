using System;
using System.ComponentModel.DataAnnotations;
#if __MOBILE__
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
#endif

namespace Distributr.Core.Domain.Master.ProductEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public class ProductGroupDiscount:MasterEntity
    {
       public ProductGroupDiscount() : base(default(Guid)) { }

       public ProductGroupDiscount(Guid id):base(id)
       {
          
       }
       public ProductGroupDiscount(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive)
       {

       }

    #if __MOBILE__
       [ForeignKey(typeof(DiscountGroup))]
       public Guid DiscountGroupMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
       public DiscountGroup GroupDiscount { get; set; }
       public decimal CurrentDiscount()
       {
           return DiscountRate;
           //var item = LatestGroupDiscountItem().FirstOrDefault(s => s.Product.ProductId == productId && s.Quantity <= quantity);
           //return item != null ? item.DiscountRate : 0;
       }
     
       public decimal DiscountRate { get; set; }

#if __MOBILE__       
       public Guid LineItemId { get; set; }

       public Guid ProductMasterId { get; set; }

       [Ignore]
#endif
       [Required]
       public ProductRef Product { get; set; }
         [Required]
       public DateTime EffectiveDate { get; set; }
         [Required]
       public DateTime EndDate { get; set; }
       public bool IsByQuantity { get; set; }
       public decimal Quantity { get; set; }
      


    }
}
