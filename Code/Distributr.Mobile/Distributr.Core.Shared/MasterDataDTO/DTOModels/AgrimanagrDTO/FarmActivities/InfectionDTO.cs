using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.FarmActivities
{
    public class InfectionDTO:MasterBaseDTO
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int InfectionTypeId { get; set; }
        public string Description { get; set; }
    }
}
