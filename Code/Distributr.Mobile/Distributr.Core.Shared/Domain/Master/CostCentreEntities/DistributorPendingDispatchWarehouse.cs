using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class DistributorPendingDispatchWarehouse : Warehouse
    {
        public DistributorPendingDispatchWarehouse(Guid id) : base(id) { }

        public DistributorPendingDispatchWarehouse(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive) { }

        
    }
}
