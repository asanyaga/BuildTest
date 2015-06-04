using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre
{
    public class RegionDTO : MasterBaseDTO
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Guid CountryMasterId { get; set; }
    }
}
