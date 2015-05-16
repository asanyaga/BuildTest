using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Agrimanagr.HQ.Areas.Agrimanagr.ViewModels
{
    public class InfectionViewModel:MasterBaseDTO
    {
        [Required]
        public string Code { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage = "Infection Type is a required field")]
        public int InfectionTypeId { get; set; }
        public string Description { get; set; }
    }
}