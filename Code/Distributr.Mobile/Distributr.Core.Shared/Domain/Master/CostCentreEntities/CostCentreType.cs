using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public enum CostCentreType : int
    {
        None = 0,
        Producer = 1,
        Distributor = 2,
        Transporter = 3,
        DistributorSalesman = 4,
        Outlet = 5,
        DistributorPendingDispatchWarehouse = 6,
        DistributorPendingReturnsWarehouse = 7,
        Hub = 8,
        CommoditySupplier = 9,
        PurchasingClerk=10,
        Store = 11
    }
}
