using System;
#if __MOBILE__
using SQLiteNetExtensions.Attributes;
using SQLite.Net.Attributes;
#endif

namespace Distributr.Core.Domain.Master.ProductEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public class FreeOfChargeDiscount:MasterEntity
    {
       public FreeOfChargeDiscount() : base(default(Guid)) { }
		
       public FreeOfChargeDiscount(Guid id):base(id)
       {

       }
       public FreeOfChargeDiscount(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive)
       {

       }

    #if __MOBILE__
       [ForeignKey(typeof(Product))]
       public Guid ProductRefMasterId { get; set; }

       [Ignore]
    #endif
       public ProductRef ProductRef { get; set; }

       public bool isChecked { get; set; }
       public DateTime EndDate { get; set; }
       public DateTime StartDate { get; set; }
    }
}
