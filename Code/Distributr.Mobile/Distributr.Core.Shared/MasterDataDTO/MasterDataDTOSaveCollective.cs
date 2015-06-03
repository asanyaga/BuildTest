using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.MasterDataDTO
{
    public enum MasterDataDTOSaveCollective : int
    {
        Outlet = 1,
        Contact = 2,
        Asset = 3,
        DistributrFile=4,
        OutletVisitDay=5,
        OutletPriority = 6,
        Target = 7,
        User = 8,
        DistributrSalesman = 9,
        Route = 10,
        PasswordChange= 11,
        AppSettings = 12,
        InventorySerials = 13,
        MasterDataAllocation = 14
    }
}
