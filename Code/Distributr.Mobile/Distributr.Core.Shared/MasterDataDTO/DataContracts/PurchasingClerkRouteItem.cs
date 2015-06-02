using System;

namespace Distributr.Core.MasterDataDTO.DataContracts
{
    public class PurchasingClerkRouteItem : MasterBaseItem
    {
        public Guid RouteId { get; set; }
        public Guid PurchasingClerkCostCentreId { get; set; }
    }
}
