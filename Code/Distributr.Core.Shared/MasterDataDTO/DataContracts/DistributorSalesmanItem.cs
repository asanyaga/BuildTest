using System;

namespace Distributr.Core.MasterDataDTO.DataContracts
{
    public class DistributorSalesmanItem : CostCentreItem
    {
        public Guid RouteMasterId { get; set; }
        public string CostCentreCode { get; set; }
    }
    public class DistributorSalesmanRouteItem : MasterBaseItem
    {
        public Guid RouteMasterId { get; set; }
        public Guid CostCentreMasterId { get; set; }
    }
}
