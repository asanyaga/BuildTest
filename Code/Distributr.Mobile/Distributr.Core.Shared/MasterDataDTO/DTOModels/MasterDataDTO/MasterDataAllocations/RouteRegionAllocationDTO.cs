using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.MasterDataAllocations
{
    public class RouteRegionAllocationDTO : MasterDataAllocationDTO
    {
        public Guid RouteId { get; set; }
        public Guid RegionId { get; set; }
    }
}
