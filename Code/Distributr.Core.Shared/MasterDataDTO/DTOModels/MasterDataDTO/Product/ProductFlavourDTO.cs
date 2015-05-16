using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product
{
    public class ProductFlavourDTO : MasterBaseDTO
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public Guid ProductBrandMasterId { get; set; }
    }
}
