using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.MasterDataAllocations
{
    public class RouteCostCentreAllocationDTO : MasterDataAllocationDTO
    {
        public Guid RouteId { get; set; }
        public Guid CostCentreId { get; set; }
    }
}
