using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product
{
    public class ProductPackagingDTO : MasterBaseDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid ContainmentMasterId { get; set; }
        public Guid ReturnableProductMasterId { get; set; }
        public string Code { get; set; }
    }
}
