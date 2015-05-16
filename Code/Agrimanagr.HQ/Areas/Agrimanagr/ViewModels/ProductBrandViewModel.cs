using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Agrimanagr.HQ.Areas.Agrimanagr.ViewModels
{
    public class ProductBrandViewModel:MasterBaseDTO
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Supplier field is required ")]
        public Guid SupplierMasterId { get; set; }
    
    }
}