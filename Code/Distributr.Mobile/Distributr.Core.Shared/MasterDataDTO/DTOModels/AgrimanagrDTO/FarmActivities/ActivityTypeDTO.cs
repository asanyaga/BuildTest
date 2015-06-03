using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.FarmActivities
{
    public class ActivityTypeDTO : MasterBaseDTO
    {
        
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsInfectionsRequired { get; set; }
        public bool IsInputRequired { get; set; }
        public bool IsServicesRequired { get; set; }
        public bool IsProduceRequired { get; set; }

    }
}