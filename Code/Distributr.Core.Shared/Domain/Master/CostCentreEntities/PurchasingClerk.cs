using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.UserEntities;

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public class PurchasingClerk: InventoryInTransitWarehouse
    {
       public PurchasingClerk(Guid id) : base(id)
       {
           PurchasingClerkRoutes = new List<PurchasingClerkRoute>();
       }

       public PurchasingClerk(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive) : base(id, dateCreated, dateLastUpdated, isActive)
       {
           PurchasingClerkRoutes= new List<PurchasingClerkRoute>();
       }
       [Required(ErrorMessage = "User  is required")]
       public User User { get; set; }

       public List<PurchasingClerkRoute> PurchasingClerkRoutes { get; internal set; }
    }
#if !SILVERLIGHT
   [Serializable]
#endif
   public class PurchasingClerkRoute : MasterEntity
   {
       public PurchasingClerkRoute(Guid id) : base(id) { }
       public PurchasingClerkRoute(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive)
       {
       }
       public Route Route { get; set; }
       public CostCentreRef PurchasingClerkRef { get; set; }

   }
}
