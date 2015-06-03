using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Master.UserEntities
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public enum UserType
    {
        None = 0,
        WarehouseManager = 1,
        OutletManager = 2,
        DistributorSalesman = 3,
        ASM = 4,
        SalesRep = 5,
        Surveyor = 6,
        HQAdmin = 7,
        PurchasingClerk = 8,
        HubManager = 9,
        Clerk = 10,
        Driver = 11,
        AgriHQAdmin = 12,
        OutletUser = 13,
        Supplier = 14
    }
}
