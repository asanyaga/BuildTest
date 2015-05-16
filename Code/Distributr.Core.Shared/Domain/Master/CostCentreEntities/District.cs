using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

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
      
       [Required(ErrorMessage="Select Province")]
       public Province Province { get; set; }
       [Required(ErrorMessage="enter district name")]
       public string DistrictName { get; set; }
    }
}
