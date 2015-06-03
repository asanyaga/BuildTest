using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product
{
    public abstract class ProductDTO : MasterBaseDTO
    {

        public string Description { get; set; }
        public Guid ProductBrandMasterId { get; set; }
        public Guid ProductPackagingMasterId { get; set; }
        public Guid ProductPackagingTypeMasterId { get; set; }
       
        public string ProductCode { get; set; }
        public int? ReturnableTypeMasterId { get; set; }
        public Guid? VatClassMasterId { get; set; }
        public decimal ExFactoryPrice { get; set; }

    }
}
