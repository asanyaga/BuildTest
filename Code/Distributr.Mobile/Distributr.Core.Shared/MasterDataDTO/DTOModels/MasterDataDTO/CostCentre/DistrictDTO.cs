using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.CostCentre
{
    public class DistrictDTO : MasterBaseDTO
    {
        public string DistrictName { get; set; }


        public Guid ProvinceMasterId { get; set; }
   
    }
}
