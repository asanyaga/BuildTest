using System;

namespace Distributr.Core.MasterDataDTO.DTOModels.MasterDataDTO.DistributorTargets
{
    public class TargetDTO : MasterBaseDTO
    {
        public Guid CostCentreId { get; set; }
        public Guid TargetPeriodMasterId { get; set; }
        public decimal TargetValue { get; set; }
        public bool IsQuantityTarget { get; set; }
    }
}
