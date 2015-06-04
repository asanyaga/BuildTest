using System;
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
using Distributr.Core.Factory.Documents;
using Distributr.Core.Notifications;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Workflow.Impl.AdjustmentNote;

namespace Distributr.Core.Workflow.Impl.Orders
{
   public class OrderPosWorkflow :IOrderPosWorkflow
    {
       IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;

        private InventoryAdjustmentNoteFactory _inventoryAdjustmentNoteFactory;
        
        private InventoryAdjustmentNoteWfManager _inventoryAdjustmentNoteWfManager;
        private ICostCentreRepository _costCentreRepository;
       private IInvoiceFactory _invoiceFactory;
       private IInvoiceRepository _invoiceRepository;
        private IConfirmInvoiceWorkFlowManager _invoiceWorkFlowManager;
       private IReceiptWorkFlowManager _receiptWorkFlowManager;
       private IReceiptFactory _receiptFactory;
        private IGetDocumentReference _getDocumentReference;
       //private IAsynchronousPaymentNotificationResponseRepository _notificationRepository;

       private INotifyService _notifyService;
        public OrderPosWorkflow(INotifyService notifyService, InventoryAdjustmentNoteFactory inventoryAdjustmentNoteFactory,  InventoryAdjustmentNoteWfManager inventoryAdjustmentNoteWfManager, ICostCentreRepository costCentreRepository, IInvoiceFactory invoiceFactory, IInvoiceRepository invoiceRepository, IConfirmInvoiceWorkFlowManager invoiceWorkFlowManager, IReceiptWorkFlowManager receiptWorkFlowManager, IReceiptFactory receiptFactory, IGetDocumentReference getDocumentReference, IOutgoingCommandEnvelopeRouter commandEnvelopeRouter)
        {
          
            _inventoryAdjustmentNoteFactory = inventoryAdjustmentNoteFactory;
            
            _inventoryAdjustmentNoteWfManager = inventoryAdjustmentNoteWfManager;
            _costCentreRepository = costCentreRepository;
            _invoiceFactory = invoiceFactory;
            _invoiceRepository = invoiceRepository;
            _invoiceWorkFlowManager = invoiceWorkFlowManager;
            _receiptWorkFlowManager = receiptWorkFlowManager;
            _receiptFactory = receiptFactory;
            _getDocumentReference = getDocumentReference;
            _commandEnvelopeRouter = commandEnvelopeRouter;
           
            _notifyService = notifyService;
        }

       public void Submit(MainOrder order,BasicConfig config)
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
               // _commandRouter.RouteDocumentCommand(_item);
            }
            var editlineItemCommands = commandsToExecute.OfType<ChangeMainOrderLineItemCommand>();
            foreach (var _editeditem in editlineItemCommands)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, _editeditem));

                //_commandRouter.RouteDocumentCommand(_editeditem);
            }
            var removedLineitem = commandsToExecute.OfType<RemoveMainOrderLineItemCommand>();
            foreach (var _removeditem in removedLineitem)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, _removeditem));

            }
            var co = commandsToExecute.OfType<ConfirmCommand>().FirstOrDefault();
            if (co != null)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, co));
                //_commandRouter.RouteDocumentCommand(co);
                _notifyService.SubmitOrderSaleNotification(order);
            }
           var rco = commandsToExecute.OfType<RejectMainOrderCommand>().FirstOrDefault();
            if (rco != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, rco));

            //adjust inventory from distributor
           Guid invoiceId;
           var approvedlineItemCommands = order.GetSubOrderCommandsToExecute().OfType<ApproveOrderLineItemCommand>();

           foreach (var _item in approvedlineItemCommands)
           {
               envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, _item));
           }
            HandleApprovedCommand(order,config,out invoiceId);
            var aop = commandsToExecute.OfType<ApproveCommand>().FirstOrDefault();
            if (aop != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, aop));

            var closecommand = commandsToExecute.OfType<CloseOrderCommand>().FirstOrDefault();
            if (closecommand != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, closecommand));
            var orderpaymentInfoitem = order.GetSubOrderCommandsToExecute().OfType<AddOrderPaymentInfoCommand>();
            foreach (var paymentInfo in orderpaymentInfoitem)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, paymentInfo));
       
                
            }
            HandlePayments(order,invoiceId, config);
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);
           
        }
       private void HandlePayments(MainOrder order, Guid invoiceId,BasicConfig config)
       {
           var orderpaymentInfoitem = order.GetSubOrderCommandsToExecute().OfType<AddOrderPaymentInfoCommand>();
           foreach (var paymentInfo in orderpaymentInfoitem)
           {
               //TODO AJM resolve this section
               /*
               var notification = _notificationRepository.GetById(paymentInfo.InfoId);
               if (notification != null)
               {
                   foreach (var paymentNotificationListItem in notification.PaymentNotificationDetails.Where(s => !s.IsUsed))
                   {
                       _notificationRepository.ConfirmNotificationItem(paymentNotificationListItem.Id);
                   }
               }*/
           }
           
           if (orderpaymentInfoitem.Any(s => s.IsConfirmed && !s.IsProcessed))
           {
               if (invoiceId == Guid.Empty)
                   invoiceId = _invoiceRepository.GetInvoiceByOrderId(order.Id).Id;
               CostCentre recepient = null;
               if (order.DocumentIssuerCostCentre is DistributorSalesman)
                   recepient = order.DocumentIssuerCostCentre;
               else
                   recepient = order.DocumentRecipientCostCentre;
               string receiptRef = _getDocumentReference.GetDocReference("Rpt", recepient.Id,order.IssuedOnBehalfOf.Id);
               Receipt receipt = _receiptFactory.Create(order.DocumentIssuerCostCentre,
                                                        order.DocumentIssuerCostCentreApplicationId,
                                                        order.DocumentRecipientCostCentre,
                                                        order.DocumentIssuerUser, receiptRef, order.Id, invoiceId,
                                                        Guid.Empty);

               foreach (var info in orderpaymentInfoitem.Where(s => s.IsConfirmed && !s.IsProcessed))
               {
                   receipt.AddLineItem(_receiptFactory.CreateLineItem(info.ConfirmedAmount, info.PaymentRefId,
                                                                      info.MMoneyPaymentType,
                                                                      info.NotificationId, 0,
                                                                      (PaymentMode)info.PaymentModeId, info.Description, receipt.Id,
                                                                      info.IsConfirmed)
                       );
               }
               receipt.Confirm();
               _receiptWorkFlowManager.SubmitChanges(receipt,config);

           }

           
       }

       private void HandleApprovedCommand(MainOrder order ,BasicConfig config,  out Guid invoiceId)
       {
           

           invoiceId = Guid.Empty;
           var approvedlineItemCommands = order.GetSubOrderCommandsToExecute().OfType<ApproveOrderLineItemCommand>();

           if (!approvedlineItemCommands.Any())
               return;
           CostCentre issuer = _costCentreRepository.GetById(config.CostCentreId);
           CostCentre recepient = null;
           if (order.DocumentIssuerCostCentre is DistributorSalesman)
               recepient = order.DocumentIssuerCostCentre;
           else
               recepient = order.DocumentRecipientCostCentre;
           InventoryAdjustmentNote inventoryAdjustmentNote = 
               _inventoryAdjustmentNoteFactory.Create(issuer,
                                                       config.CostCentreApplicationId,
                                                        recepient,
                                                        order.DocumentIssuerUser,
                                                        order.DocumentReference,
                                                        InventoryAdjustmentNoteType.Available, 
                                                        order.ParentId);
           inventoryAdjustmentNote.DocumentParentId = order.Id;
           string invoiceRef = _getDocumentReference.GetDocReference("Inv",recepient.Id,order.IssuedOnBehalfOf.Id);
        
           Domain.Transactional.DocumentEntities.Invoice invoice = _invoiceFactory.Create(issuer, config.CostCentreApplicationId, recepient,
                                                    order.DocumentIssuerUser, invoiceRef, order.Id, order.Id);
           invoiceId = invoice.Id;
           
           foreach (var _item in approvedlineItemCommands)
           {
               ApproveOrderLineItemCommand ap = _item as ApproveOrderLineItemCommand;
               SubOrderLineItem soli = order.PendingApprovalLineItems.First(s => s.Id == ap.LineItemId);
               if (ap.ApprovedQuantity > 0)
               {
                   inventoryAdjustmentNote.AddLineItem(_inventoryAdjustmentNoteFactory.CreateLineItem(0,soli.Product.Id, ap.ApprovedQuantity,0, "Inventory adjustment"));
                   invoice.AddLineItem(_invoiceFactory.CreateLineItem(soli.Product.Id, ap.ApprovedQuantity, soli.Value, order.DocumentReference, 0, soli.LineItemVatValue, soli.ProductDiscount, soli.DiscountType));
               }
             //  _commandRouter.RouteDocumentCommand(_item);
           }
           inventoryAdjustmentNote.Confirm();
           _inventoryAdjustmentNoteWfManager.SubmitChanges(inventoryAdjustmentNote,config);
           invoice.Confirm();
           
           _invoiceWorkFlowManager.SubmitChanges(invoice,config);

           
               
               
           }
    }
}