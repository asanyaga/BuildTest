using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;

namespace Distributr.Core.Workflow.Impl.Orders
{
    public class PurchaseOrderWorkflow:IPurchaseOrderWorkflow
    {
        IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;
      

        public PurchaseOrderWorkflow(  IOutgoingCommandEnvelopeRouter commandEnvelopeRouter)
        {
           
            _commandEnvelopeRouter = commandEnvelopeRouter;
        }

        public void Submit(MainOrder order)
        {
            int sequence = 0;
            var envelope = new CommandEnvelope();
            envelope.Initialize(order);
           
            List<DocumentCommand> commandsToExecute = order.GetSubOrderCommandsToExecute();
            var createCommand = commandsToExecute.OfType<CreateCommand>().FirstOrDefault();
            if (createCommand != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, createCommand));
            var lineItemCommands = commandsToExecute.OfType<AfterCreateCommand>();
            foreach (var _item in lineItemCommands)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, _item));
            }
            var editlineItemCommands = commandsToExecute.OfType<ChangeMainOrderLineItemCommand>();
            foreach (var _editeditem in editlineItemCommands)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, _editeditem));
            }
            var removedLineitem = commandsToExecute.OfType<RemoveMainOrderLineItemCommand>();
            foreach (var _removeditem in removedLineitem)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, _removeditem));
            }
            var co = commandsToExecute.OfType<ConfirmCommand>().FirstOrDefault();
            if (co != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, co));
            var rco = commandsToExecute.OfType<RejectMainOrderCommand>().FirstOrDefault();
            if (rco != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, rco));

            var approvedlineItemCommands = order.GetSubOrderCommandsToExecute().OfType<ApproveOrderLineItemCommand>();
            foreach (var _item in approvedlineItemCommands)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, _item));
            }

            var aop = commandsToExecute.OfType<ApproveCommand>().FirstOrDefault();
            if (aop != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, aop));
            var closecommand = commandsToExecute.OfType<CloseOrderCommand>().FirstOrDefault();
            if (closecommand != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, closecommand));
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);
        }
    }
}
