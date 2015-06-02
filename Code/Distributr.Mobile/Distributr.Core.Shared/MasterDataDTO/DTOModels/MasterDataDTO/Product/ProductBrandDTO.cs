using System;
using System.ComponentModel.DataAnnotations;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.Product
{
    public class ProductBrandDTO : MasterBaseDTO
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Supplier field is required ")]
        public Guid SupplierMasterId { get; set; }
    }
}
