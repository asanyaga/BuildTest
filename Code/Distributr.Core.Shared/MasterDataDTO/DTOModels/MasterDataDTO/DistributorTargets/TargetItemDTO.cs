using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.DistributorTargets
{
    public class TargetItemDTO: MasterBaseDTO
    {
        public Guid ProductMasterId { get; set; }
        public decimal Quantity { get; set; }
        public Guid TargetMasterId { get; set; }
    }
}
