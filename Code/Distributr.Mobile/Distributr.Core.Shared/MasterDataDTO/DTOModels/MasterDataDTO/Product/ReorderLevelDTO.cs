using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product
{
    public class ReorderLevelDTO : MasterBaseDTO
    {
        public Guid DistributorMasterId { get; set; }
        public Guid ProductMasterId { get; set; }
        public decimal ProductReOrderLevel { get; set; }
    }
}
