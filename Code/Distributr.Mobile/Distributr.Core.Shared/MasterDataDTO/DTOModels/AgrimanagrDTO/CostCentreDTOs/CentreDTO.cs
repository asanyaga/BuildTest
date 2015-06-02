using System;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;

namespace Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CostCentreDTOs
{
    public class CentreDTO : MasterBaseDTO
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CenterTypeId { get; set; }
        public Guid HubId { get; set; }
        public Guid RouteId { get; set; }
    }
}
