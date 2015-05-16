using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.ClientApp.CommandResults;

namespace Distributr.WSAPI.Lib.ResponseBuilders.CommandRouter
{
    [Obsolete("Command Envelope Refactoring")]
    public interface ICommandRouterResponseBuilder
    {
        DocumentCommandRoutingResponse GetNextDocumentCommand(Guid costCentreApplicationId,Guid costCentreId, long lastDeliveredCommandRouteItemId);
        BatchDocumentCommandRoutingResponse GetNextBatcDocumentCommand(Guid costCentreApplicationId, Guid costCentreId, List<long> lastDeliveredCommandRouteItemIds, int batchSize);
    }
}
