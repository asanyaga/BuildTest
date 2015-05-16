using System;
using System.Collections.Generic;
using Distributr.Core.ClientApp.CommandResults;

namespace Distributr.WSAPI.Lib.ResponseBuilders.CommandRouter
{
    public interface ICommandEnvelopeRouterResponseBuilder
    {
         BatchDocumentCommandEnvelopeRoutingResponse GetNextCommandEnvelopes(Guid costCentreApplicationId, Guid costCentreId, List<Guid> lastDeliveredEnvelopeIds, int batchSize);
         BatchDocumentCommandEnvelopeRoutingResponse GetNextInventoryCommandEnvelopes(Guid costCentreApplicationId, Guid costCentreId, List<Guid> lastDeliveredEnvelopeIds, int batchSize);
    }
}