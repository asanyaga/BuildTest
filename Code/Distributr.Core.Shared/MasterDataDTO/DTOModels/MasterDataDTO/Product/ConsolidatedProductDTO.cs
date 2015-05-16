using System;
using System.Collections.Generic;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product
{
    public class ConsolidatedProductDTO : ProductDTO
    {
        public List<ConsolidatedProductProductDetailDTO> ProductDetails { get; set; }
    }

    public class ConsolidatedProductProductDetailDTO : MasterBaseDTO
    {
        public Guid ProductDetailProductMasterId { get; set; }
        public Guid ConsolidatedProductId { get; set; }
        public int ProductDetailQuantityPerConsolidatedProduct { get; set; }
    }
}
