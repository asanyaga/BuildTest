using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product
{
    public class ContainmentDTO : MasterBaseDTO
    {
        public int Quantity { get; set; }
        public Guid ReturnableProductMasterId { get; set; }
        public Guid ProductPackagingTypeMasterId { get; set; }
    }
}
