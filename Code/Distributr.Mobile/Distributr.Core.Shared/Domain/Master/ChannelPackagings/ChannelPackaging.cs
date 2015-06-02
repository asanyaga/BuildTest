using System;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
#if __MOBILE__
using SQLiteNetExtensions.Attributes;
#endif

namespace Distributr.Core.Domain.Master.ChannelPackagings
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public class ChannelPackaging:MasterEntity
    {
       public ChannelPackaging() : base(default(Guid)) { }

       public ChannelPackaging(Guid id)
           : base(id)
       {
           
       }
       public ChannelPackaging(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive)
       { }

    #if __MOBILE__
       [ForeignKey(typeof(ProductPackaging))]
       public Guid ProductPackagingMasterId { get; set; }
       
       [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
       public ProductPackaging Packaging { get; set; }

    #if __MOBILE__
       [ForeignKey(typeof(OutletType))]
       public Guid OutletTypeMasterId { get; set; }

       [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
       public OutletType OutletType { get; set; }
       public bool IsChecked { get; set; }
       
    }
}
