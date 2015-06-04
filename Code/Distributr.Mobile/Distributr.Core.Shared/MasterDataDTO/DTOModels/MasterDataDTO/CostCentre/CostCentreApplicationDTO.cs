using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre
{
    public class CostCentreApplicationDTO : MasterBaseDTO
    {
        public Guid CostCentreId { get; set; }
        public string Description { get; set; }
    }
}
