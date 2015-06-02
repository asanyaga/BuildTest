using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre
{
    public class SalesmanRouteDTO: MasterBaseDTO
    {
        public Guid RouteMasterId { get; set; }
        public Guid DistributorSalesmanMasterId { get; set; }
    }
}
