using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre
{
    public class ProvinceDTO : MasterBaseDTO
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Guid CountryMasterId { get; set; }
    }
}
