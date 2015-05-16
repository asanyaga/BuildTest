using System;
using System.Collections.Generic;
using Distributr.Core.Commands.CommandPackage;

namespace Distributr.WSAPI.Lib.Services.Routing
{
    public interface ICommandEnvelopeRouteOnRequestCostcentreRepository
    {
        void AddCommandEnvelopeRouteCentre(CommandEnvelope envelope);
        void AddCommandEnvelopeRoutingStatus(CommandEnvelopeRoutingStatus commandEnvelopeRoutingStatus);
        void MarkEnvelopesAsDelivered(List<Guid> envelopesIdList,Guid costCentreApplicationId,Guid costCentreId);

        List<CommandEnvelope> GetUnDeliveredEnvelopesByDestinationCostCentreApplicationId(Guid costCentreApplicationId,
            Guid costCentreId, int batchSize, bool includeArchived);
        List<CommandEnvelope> GetUnDeliveredInventoryEnvelopesByDestinationCostCentreApplicationId(Guid costCentreApplicationId,
           Guid costCentreId, int batchSize, bool includeArchived);
      
        void MarkEnvelopeAsInvalid(Guid costCentreId);
        void RetireEnvelopes(Guid documentId);
        void UpdateStatus();
    }
}