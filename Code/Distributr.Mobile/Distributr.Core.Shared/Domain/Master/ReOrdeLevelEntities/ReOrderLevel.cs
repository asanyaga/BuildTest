
using System;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
#if __MOBILE__
using SQLiteNetExtensions.Attributes;
using SQLite.Net.Attributes;
#endif

namespace Distributr.Core.Domain.Master.ReOrdeLevelEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
#if __MOBILE__
   [Table("ReorderLevel")]
#endif
   public class ReOrderLevel:MasterEntity
    {
       public ReOrderLevel(Guid id)
           : base(id)
       { 
       
       }
       public ReOrderLevel(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive)
       { 
       
       }

    #if __MOBILE__
       [ForeignKey(typeof(Distributor))]
       public Guid DistributorMasterId { get; set; }

       [OneToOne(CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
       public Distributor Distributor { get; set; }

       [Ignore]
    #endif
       public CostCentre DistributorId { get; set; }
    
    #if __MOBILE__
       [ForeignKey(typeof(Product))]
       public Guid ProductMasterId { get; set; }

       [OneToOne(CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]
    #endif
       public Product ProductId { get; set; }
    
       public decimal ProductReOrderLevel { get; set; }
    }
}
