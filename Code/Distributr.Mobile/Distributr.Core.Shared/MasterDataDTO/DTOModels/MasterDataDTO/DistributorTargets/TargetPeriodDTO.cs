using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.DistributorTargets
{
    public class TargetPeriodDTO : MasterBaseDTO
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
