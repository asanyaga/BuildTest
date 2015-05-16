using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Agrimanagr.HQ.Areas.Agrimanagr.ViewModels
{
    public class ProductFlavourViewModel : MasterBaseDTO
    {

        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Product Brand field is Required")]
        public Guid ProductBrandMasterId { get; set; }
    }
}