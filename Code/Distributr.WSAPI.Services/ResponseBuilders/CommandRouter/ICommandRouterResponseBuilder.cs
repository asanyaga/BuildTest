using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.ClientApp.CommandResults;

namespace Distributr.WSAPI.Lib.ResponseBuilders.CommandRouter
{
    public interface ICommandRouterResponseBuilder
    {
        DocumentCommandRoutingResponse GetNextDocumentCommand(Guid costCentreApplicationId, long lastDeliveredCommandRouteItemId);
        BatchDocumentCommandRoutingResponse GetNextBatcDocumentCommand(Guid costCentreApplicationId, List<long> lastDeliveredCommandRouteItemIds, int batchSize);
    }
}
