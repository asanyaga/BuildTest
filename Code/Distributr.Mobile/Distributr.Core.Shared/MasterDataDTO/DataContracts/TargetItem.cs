using System;

namespace Distributr.Core.MasterDataDTO.DataContracts
{
    public class CostCentreTargetItem : MasterBaseItem
    {
        public Guid CostCentreMasterId { get; set; }

        public Guid TargetPeriodMasterId { get; set; }

        public decimal TargetValue { get; set; }

        public bool IsQuantityTarget { get; set; }
    }
}
