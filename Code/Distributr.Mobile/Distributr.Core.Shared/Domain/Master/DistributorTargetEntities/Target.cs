using System;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master.CostCentreEntities;
#if __MOBILE__
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
#endif


namespace Distributr.Core.Domain.Master.DistributorTargetEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public class Target:MasterEntity
    {
       public Target() : base(default(Guid)) { }
       public Target(Guid id):base(id)
       {
        
       }
       public Target(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive)
       { 
       
       }
    #if __MOBILE__
       public Guid CostCentreId { get; set; }

       [Ignore]
    #endif  
       [Required(ErrorMessage = "Cost Centre Is Required")]
       public CostCentre CostCentre { get; set; }
       //public Product product { get; set; }

#if __MOBILE__
       [ForeignKey(typeof(TargetPeriod))]
       public Guid TargetPeriodMasterId { get; set; }

       [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
#endif  
       public TargetPeriod TargetPeriod { get; set; }
       public decimal TargetValue { get; set; }
       public bool IsQuantityTarget { get; set; }
    }

   
}
