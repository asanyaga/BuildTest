using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.MasterDataAllocations
{
    public class RouteCentreAllocationDTO : MasterDataAllocationDTO
    {
        public Guid RouteId { get; set; }
        public Guid CentreId { get; set; }
    }
}
