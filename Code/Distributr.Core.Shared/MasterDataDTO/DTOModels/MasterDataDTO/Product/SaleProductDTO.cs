using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product
{
    public class 
        SaleProductDTO : ProductDTO
    {
        public Guid ProductFlavourMasterId { get; set; }

        public Guid ProductTypeMasterId { get; set; }

        public Guid ReturnableProductMasterId { get; set; }
        public Guid ReturnableContainerMasterId { get; set; }
        public int ContainerCapacity { get; set; }
    }
}
