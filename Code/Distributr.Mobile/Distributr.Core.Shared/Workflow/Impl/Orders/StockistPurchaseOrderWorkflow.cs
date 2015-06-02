using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Factory.Documents.Impl;
using Distributr.Core.Repository.Master.CostCentreRepositories;

namespace Distributr.Core.Workflow.Impl.Orders
{
    public class StockistPurchaseOrderWorkflow : IStockistPurchaseOrderWorkflow
    {
        private IOutgoingDocumentCommandRouter _commandRouter;
        private InventoryTransferNoteFactory _inventoryTransferNoteFactory;
        private IConfirmInventoryTransferNoteWFManager _inventoryTransferNoteWfManager;
        private ICostCentreRepository _costCentreRepository;
        private IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;

        public StockistPurchaseOrderWorkflow(IOutgoingDocumentCommandRouter commandRouter, InventoryTransferNoteFactory inventoryTransferNoteFactory, IConfirmInventoryTransferNoteWFManager inventoryTransferNoteWfManager, ICostCentreRepository costCentreRepository, IOutgoingCommandEnvelopeRouter commandEnvelopeRouter)
        {
            _commandRouter = commandRouter;
           
            _inventoryTransferNoteFactory = inventoryTransferNoteFactory;
            _inventoryTransferNoteWfManager = inventoryTransferNoteWfManager;
            _costCentreRepository = costCentreRepository;
            _commandEnvelopeRouter = commandEnvelopeRouter;
        }

        public void Submit(MainOrder order,BasicConfig config)
        {
            int sequence = 0;
            var envelope = new CommandEnvelope();
            envelope.Initialize(order);
            List<DocumentCommand> commandsToExecute = order.GetSubOrderCommandsToExecute();
            var createCommand = commandsToExecute.OfType<CreateCommand>().FirstOrDefault();
            if (createCommand != null)
                //_commandRouter.RouteDocumentCommand(createCommand);
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, createCommand));
            var lineItemCommands = commandsToExecute.OfType<AfterCreateCommand>();
            foreach (var _item in lineItemCommands)
            {
                //_commandRouter.RouteDocumentCommand(_item);
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, _item));
            }
            var editlineItemCommands = commandsToExecute.OfType<ChangeMainOrderLineItemCommand>();
            foreach (var _editeditem in editlineItemCommands)
            {
                //_commandRouter.RouteDocumentCommand(_editeditem);
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, _editeditem));
            }
            var removedLineitem = commandsToExecute.OfType<RemoveMainOrderLineItemCommand>();
            foreach (var _removeditem in removedLineitem)
            {
                //_commandRouter.RouteDocumentCommand(_removeditem);
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, _removeditem));
            }
            var co = commandsToExecute.OfType<ConfirmCommand>().FirstOrDefault();
            if (co != null)
            {
                //_commandRouter.RouteDocumentCommand(co);
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, co));
            }
            var rco = commandsToExecute.OfType<RejectMainOrderCommand>().FirstOrDefault();
            if (rco != null)
            {
                //_commandRouter.RouteDocumentCommand(rco);
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, rco));
            }

            var approvedlineItemCommands = order.GetSubOrderCommandsToExecute().OfType<ApproveOrderLineItemCommand>();
            foreach (var _item in approvedlineItemCommands)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, _item));
                //_commandRouter.RouteDocumentCommand(_item);
            }

            var aop = commandsToExecute.OfType<ApproveCommand>().FirstOrDefault();
            if (aop != null)
            {
                //_commandRouter.RouteDocumentCommand(aop);
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, aop));
            }
            var closecommand = commandsToExecute.OfType<CloseOrderCommand>().FirstOrDefault();
            if (closecommand != null)
            {
                //_commandRouter.RouteDocumentCommand(closecommand);
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, closecommand));
            }

            HandleApprovedCommand(order, config);
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);
        }

        private void HandleApprovedCommand(MainOrder order,BasicConfig config)
        {
            
          
            var approvedlineItemCommands = order.GetSubOrderCommandsToExecute().OfType<ApproveOrderLineItemCommand>();

            if (!approvedlineItemCommands.Any())
                return;
            CostCentre issuer = _costCentreRepository.GetById(config.CostCentreId);
            CostCentre recepient = null;
            if (order.DocumentIssuerCostCentre is DistributorSalesman)
                recepient = order.DocumentIssuerCostCentre;
            else
                recepient = order.DocumentRecipientCostCentre;
            InventoryTransferNote inventoryTransfernote = _inventoryTransferNoteFactory.Create(issuer,
                                                                              config.CostCentreApplicationId,
                                                                              order.DocumentIssuerUser,
                                                                              recepient,
                                                                              order.IssuedOnBehalfOf,
                                                                              order.DocumentReference);
            inventoryTransfernote.DocumentParentId = inventoryTransfernote.Id;
           
            foreach (var _item in approvedlineItemCommands)
            {
                ApproveOrderLineItemCommand ap = _item as ApproveOrderLineItemCommand;
                SubOrderLineItem soli = order.PendingApprovalLineItems.First(s => s.Id == ap.LineItemId);
                if (ap.ApprovedQuantity > 0)
                {
                    inventoryTransfernote.AddLineItem(_inventoryTransferNoteFactory.CreateLineItem(soli.Product.Id, ap.ApprovedQuantity, 0, 0, order.DocumentReference));

                }
                _commandRouter.RouteDocumentCommand(_item);
            }
            inventoryTransfernote.Confirm();
            _inventoryTransferNoteWfManager.SubmitChanges(inventoryTransfernote,config);

        }
    }
}