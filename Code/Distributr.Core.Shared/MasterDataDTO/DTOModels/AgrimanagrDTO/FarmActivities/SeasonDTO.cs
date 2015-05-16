using System;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.FarmActivities
{
    public class SeasonDTO:MasterBaseDTO
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Guid CommodityProducerId { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
