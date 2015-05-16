using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.WSAPI.Lib.Services.Routing
{
    public interface ICommandRoutingOnRequestRepository
    {
        CommandRouteOnRequest GetById(long id);
        CommandRouteOnRequest GetByCommandId(Guid id);
        CommandRouteOnRequest GetByDocumentId(Guid id);
        CommandRouteOnRequest GetUndeliveredByDestinationCostCentreApplicationId(Guid costCentreApplicationId);
        long Add(CommandRouteOnRequest commandRouteItem);
        void MarkAsDelivered(long commandId, Guid costCentreApplicationId);
        void MardAdDeliveredAndExecuted(long commandId, Guid costCentreApplicationId);
        void MarkBatchAsDelivered(List<long> commandId, Guid costCentreApplicationId);
        List<CommandRouteOnRequest> GetUnexecutedBatchByDestinationCostCentreApplicationId(Guid costCentreApplicationId, int batchSize);
        void RetireCommands(Guid parentCommandId);
        void MarkCommandsAsInvalid(Guid costCentreId);
        void CleanApplication(Guid applicationId);
    }
}
