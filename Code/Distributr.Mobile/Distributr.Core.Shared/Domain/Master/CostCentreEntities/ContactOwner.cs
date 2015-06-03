using System;

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public enum ContactOwnerType : int
    {
        None = 0,
        Distributor = 1,
        User = 2,
       
#if(KEMSA)
        HealthFacility = 3
#else
       Outlet=3,
#endif
       CommoditySupplier=4
    }
}
