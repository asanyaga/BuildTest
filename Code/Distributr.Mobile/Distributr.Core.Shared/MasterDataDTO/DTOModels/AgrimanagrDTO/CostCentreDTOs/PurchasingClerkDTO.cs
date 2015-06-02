using System;
using System.Collections.Generic;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre;
using Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.User;

namespace Distributr.Core.MasterDataDTO.DTOModels.AgrimanagrDTO.CostCentreDTOs
{
    public class PurchasingClerkDTO : CostCentreDTO
    {
        public Guid UserId { get; set; }
        public UserDTO UserDto { get; set; }
        public List<PurchasingClerkRouteDTO> PurchasingClerkRoutes { get; set; }
    }

    public class PurchasingClerkRouteDTO : MasterBaseDTO
    {
        public Guid RouteId { get; set; }
        public Guid PurchasingClerkId { get; set; }
    }
}
