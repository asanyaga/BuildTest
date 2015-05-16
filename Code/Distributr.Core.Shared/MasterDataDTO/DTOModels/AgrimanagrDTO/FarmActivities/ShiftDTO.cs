using System;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.FarmActivities
{
    public class ShiftDTO:MasterBaseDTO
    {
        [Required]
       public string Code { get; set; }
       public string Name { get; set; }
       public DateTime StartTime { get; set; }
       public DateTime EndTime { get; set; }
       public string StartTimeString { get; set; }
       public string EndTimeString { get; set; }
       public string Description { get; set; }
    }
}
