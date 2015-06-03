using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public abstract class InventoryInTransitWarehouse : Warehouse
    {
        internal InventoryInTransitWarehouse(Guid id) : base(id)
        {

        }
        public InventoryInTransitWarehouse(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }
    }
}
