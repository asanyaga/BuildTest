using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.MasterDataAllocations
{
    public class CommodityProducerCentreAllocationDTO : MasterDataAllocationDTO
    {
        public Guid CommodityProducerId { get; set; }
        public Guid CentreId { get; set; }
    }
}
