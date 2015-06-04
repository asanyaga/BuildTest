using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product
{
    public class ReturnableProductDTO : ProductDTO
    {
        public Guid ProductFlavourMasterId { get; set; }
        public int Capacity { get; set; }
        public Guid ReturnableProductMasterId { get; set; }
    }
}
