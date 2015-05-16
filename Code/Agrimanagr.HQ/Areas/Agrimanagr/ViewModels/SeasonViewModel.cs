using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Agrimanagr.HQ.Areas.Agrimanagr.ViewModels
{
    public class SeasonViewModel : MasterBaseDTO
    {
        public string Code { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage = "Commodity Producer is a required field")]
        public Guid CommodityProducerId { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}