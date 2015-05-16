using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.Recollections;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.Recollections;

namespace Distributr.Core.Workflow.Impl.ReCollections
{
    public class ReCollectionWFManager : IReCollectionWFManager
    {
        IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;

        public ReCollectionWFManager(IOutgoingCommandEnvelopeRouter commandEnvelopeRouter)
        {
            _commandEnvelopeRouter = commandEnvelopeRouter;
        }


        public void SubmitChanges(ReCollection document)
        {
            int sequence = 0;
            var envelope = new CommandEnvelope();
            envelope.Id = Guid.NewGuid();
            envelope.EnvelopeGeneratedTick = DateTime.Now.Ticks;
            envelope.GeneratedByCostCentreId = document.CostCentreId;
            envelope.RecipientCostCentreId = document.RecepientCostCentreId;
            envelope.DocumentTypeId = (int)DocumentType.RecollectionNote;
            envelope.GeneratedByCostCentreApplicationId = document.CostCentreApplicationId;
            envelope.ParentDocumentId = document.Id;
            envelope.DocumentId = document.Id;
            List<DocumentCommand> commandsToExecute = document.GetRecollectionCommandsToExecute();
            var lineItemCommands = commandsToExecute.OfType<ReCollectionCommand>();
            foreach (var _item in lineItemCommands)
            {
                
                    envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, _item));
            }
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);
        }
    }
}
