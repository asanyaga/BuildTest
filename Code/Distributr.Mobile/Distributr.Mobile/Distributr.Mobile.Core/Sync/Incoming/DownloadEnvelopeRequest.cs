using System;
using System.Collections.Generic;

namespace Distributr.Mobile.Core.Sync.Incoming
{
    public class DownloadEnvelopeRequest
    {
        public DownloadEnvelopeRequest(List<Guid> deliveredEnvelopeIds, string costCentreApplicationId)
        {
            DeliveredEnvelopeIds = deliveredEnvelopeIds;
            CostCentreApplicationId = costCentreApplicationId;
        }

        public List<Guid> DeliveredEnvelopeIds { get; set; }
        public string CostCentreApplicationId { get; set; }
        public int BatchSize { get { return 1; } }

        public DownloadEnvelopeRequest ButWithGuid(Guid lastEnvelopeId)
        {
            return new DownloadEnvelopeRequest(new List<Guid>() {lastEnvelopeId}, CostCentreApplicationId);
        }
    }
}
