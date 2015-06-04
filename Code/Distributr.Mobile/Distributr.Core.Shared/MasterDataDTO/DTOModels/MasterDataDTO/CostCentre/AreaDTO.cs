using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre
{
    public class AreaDTO : MasterBaseDTO
    {
        public string Name { get; set; }

        public string Description { get; set; }
        
        public Guid RegionMasterId { get; set; }
    }
}
