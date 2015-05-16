using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;

namespace Distributr.Core.Domain.Master.ReOrdeLevelEntities
{
#if !SILVERLIGHT
   [Serializable]
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
       public CostCentre DistributorId { get; set; }
       public Product ProductId { get; set; }
       public decimal ProductReOrderLevel { get; set; }
    }
}
