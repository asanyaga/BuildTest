using System;
using System.ComponentModel.DataAnnotations;
#if __MOBILE__
using SQLiteNetExtensions.Attributes;
#endif

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public class District:MasterEntity
    {
       public District(Guid id)
           : base(id)
       {
       
       }
       public District(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive)
       { 
       }

#if __MOBILE__
       [ForeignKey(typeof(Province))]
       public Guid ProvinceMasterId { get; set; }

       [OneToOne(CascadeOperations = CascadeOperation.CascadeInsert | CascadeOperation.CascadeRead)]      
#endif
       [Required(ErrorMessage="Select Province")]
       public Province Province { get; set; }
      
       [Required(ErrorMessage="enter district name")]
       public string DistrictName { get; set; }
    }
}
