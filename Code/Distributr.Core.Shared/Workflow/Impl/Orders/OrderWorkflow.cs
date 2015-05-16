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
using Distributr.Core.Factory.Documents.Impl;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;

namespace Distributr.Core.Workflow.Impl.Orders
{
    public class OrderWorkflow : IOrderWorkflow
    {
       // private IOutgoingDocumentCommandRouter _commandRouter;
        private InventoryTransferNoteFactory _inventoryTransferNoteFactory;
        private IConfirmInventoryTransferNoteWFManager _inventoryTransferNoteWfManager;
        private ICostCentreRepository _costCentreRepository;
        
       
        private IDispatchNoteFactory _dispatchNoteFactory;
        private IConfirmDispatchNoteWFManager _dispatchNoteWfManager;
        private IInvoiceFactory _invoiceFactory;
        private IConfirmInvoiceWorkFlowManager _invoiceWorkFlowManager;
        private IGetDocumentReference _getDocumentReference;
        private IReceiptWorkFlowManager _receiptWorkFlowManager;
        private IReceiptFactory _receiptFactory;
        private ICreditNoteFactory _creditNoteFactory;
        private IConfirmCreditNoteWFManager _confirmCreditNoteWFManager;
        private IInvoiceRepository _invoiceRepository;
       
        private IOutgoingCommandEnvelopeRouter _commandEnvelopeRouter;
       
        public OrderWorkflow( IInvoiceRepository invoiceRepository, InventoryTransferNoteFactory inventoryTransferNoteFactory,  IConfirmInventoryTransferNoteWFManager inventoryTransferNoteWfManager, ICostCentreRepository costCentreRepository, IDispatchNoteFactory dispatchNoteFactory, IConfirmDispatchNoteWFManager dispatchNoteWfManager, IInvoiceFactory invoiceFactory, IConfirmInvoiceWorkFlowManager invoiceWorkFlowManager, IGetDocumentReference getDocumentReference, IReceiptWorkFlowManager receiptWorkFlowManager, IReceiptFactory receiptFactory, ICreditNoteFactory creditNoteFactory, IConfirmCreditNoteWFManager confirmCreditNoteWfManager, IOutgoingCommandEnvelopeRouter commandEnvelopeRouter)
        {
           // _commandRouter = commandRouter;
            _inventoryTransferNoteFactory = inventoryTransferNoteFactory;
           
            _inventoryTransferNoteWfManager = inventoryTransferNoteWfManager;
            _costCentreRepository = costCentreRepository;
            _dispatchNoteFactory = dispatchNoteFactory;
            _dispatchNoteWfManager = dispatchNoteWfManager;
            _invoiceFactory = invoiceFactory;
            _invoiceWorkFlowManager = invoiceWorkFlowManager;
            _getDocumentReference = getDocumentReference;
            _receiptWorkFlowManager = receiptWorkFlowManager;
            _receiptFactory = receiptFactory;
            _creditNoteFactory = creditNoteFactory;
            _confirmCreditNoteWFManager = confirmCreditNoteWfManager;
            _commandEnvelopeRouter = commandEnvelopeRouter;
            _invoiceRepository = invoiceRepository;
           
           
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
              //  _commandRouter.RouteDocumentCommand(createCommand);
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
               // _commandRouter.RouteDocumentCommand(_editeditem);
            }
            var removedLineitem = commandsToExecute.OfType<RemoveMainOrderLineItemCommand>();
            foreach (var _removeditem in removedLineitem)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, _removeditem));
               // _commandRouter.RouteDocumentCommand(_removeditem);
            }
            var co = commandsToExecute.OfType<ConfirmCommand>().FirstOrDefault();
            if (co != null)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, co));
                //_commandRouter.RouteDocumentCommand(co);
               // _notifyService.SubmitOrderSaleNotification(order);


            }


            //transfer inventory to pending warehouse
            foreach (var _item in  order.GetSubOrderCommandsToExecute().OfType<ApproveOrderLineItemCommand>())
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, _item));
                
            }
            HandleApprovedCommand(order,config);

            var aop = commandsToExecute.OfType<ApproveCommand>().FirstOrDefault();
            if (aop != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, aop));

            // transfer inventory to pending warehouse
            var dop = order.GetSubOrderCommandsToExecute().OfType<OrderDispatchApprovedLineItemsCommand>().FirstOrDefault();
            if (dop != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, dop));
            HandleDispatchLineItemsCommand(order,config);
            var orderpaymentInfoitem = order.GetSubOrderCommandsToExecute().OfType<AddOrderPaymentInfoCommand>();
            foreach (var paymentInfo in orderpaymentInfoitem)
            {
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, paymentInfo));
                //_commandRouter.RouteDocumentCommand(paymentInfo);
                
            }
            HandlePayments(order,config);
            var closecommand = commandsToExecute.OfType<CloseOrderCommand>().FirstOrDefault();
            if (closecommand != null)
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, closecommand));
            var rco = commandsToExecute.OfType<RejectMainOrderCommand>().FirstOrDefault();
            if (rco != null)
            {
                //_commandRouter.RouteDocumentCommand(rco);
                envelope.CommandsList.Add(new CommandEnvelopeItem(++sequence, rco));
                HandleLostSale(order,config);
            }
            _commandEnvelopeRouter.RouteCommandEnvelope(envelope);
        }

        private void HandleLostSale(MainOrder order, BasicConfig config)
        {
            if(!order.IsEditable())
            {
                CostCentre salesman = null;
                if (order.DocumentIssuerCostCentre is DistributorSalesman)
                    salesman = order.DocumentIssuerCostCentre;
                else
                    salesman = order.DocumentRecipientCostCentre;
                Domain.Transactional.DocumentEntities.Invoice invoice = _invoiceRepository.GetInvoiceByOrderId(order.Id);
                string creditnoteRef=_getDocumentReference.GetDocReference("CN", salesman.Id,order.IssuedOnBehalfOf.Id);
                CreditNote cn = _creditNoteFactory
                    .Create(order.DocumentIssuerCostCentre, order.DocumentIssuerCostCentreApplicationId, order.DocumentRecipientCostCentre, order.DocumentIssuerUser, creditnoteRef, order.Id, invoice.Id);
                foreach (var _item in order.PendingApprovalLineItems)
                {
                    cn.AddLineItem(_creditNoteFactory.CreateLineItem(_item.Product.Id, _item.Qty,
                                                                           _item.Value, order.DocumentReference, 0,
                                                                           _item.LineItemVatValue, _item.ProductDiscount
                                                                           ));

                }
                cn.Confirm();
                _confirmCreditNoteWFManager.SubmitChanges(cn, config);
            }
        }

        private void HandlePayments(MainOrder order,BasicConfig config)
        {
            var orderpaymentInfoitem = order.GetSubOrderCommandsToExecute().OfType<AddOrderPaymentInfoCommand>();
            foreach (var paymentInfo in orderpaymentInfoitem)
            {
              //  _commandRouter.RouteDocumentCommand(paymentInfo);
                //TODO JUVE TONY!!!!!
                //var notification = _notificationRepository.GetById(paymentInfo.InfoId);
                //if (notification != null)
                //{
                //    foreach (var paymentNotificationListItem in notification.PaymentNotificationDetails.Where(s => !s.IsUsed))
                //    {
                //        _notificationRepository.ConfirmNotificationItem(paymentNotificationListItem.Id);
                //    }
                //}
            }
            var invoice = _invoiceRepository.GetInvoiceByOrderId(order.Id);
            if (orderpaymentInfoitem.Any(s => s.IsConfirmed && !s.IsProcessed) && invoice!= null)
            {
                Guid currentInvoice = invoice.Id;
                CostCentre salesman = null;
                if (order.DocumentIssuerCostCentre is DistributorSalesman)
                    salesman = order.DocumentIssuerCostCentre;
                else
                    salesman = order.DocumentRecipientCostCentre;
                string receiptRef = _getDocumentReference.GetDocReference("Rpt", salesman.Id,order.IssuedOnBehalfOf.Id);
                Receipt receipt = _receiptFactory.Create(order.DocumentIssuerCostCentre,
                                                         config.CostCentreApplicationId,
                                                         order.DocumentRecipientCostCentre,
                                                         order.DocumentIssuerUser, receiptRef, order.Id, currentInvoice,
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

        private void HandleDispatchLineItemsCommand(MainOrder order, BasicConfig config)
        {
            var dop = order.GetSubOrderCommandsToExecute().OfType<OrderDispatchApprovedLineItemsCommand>().FirstOrDefault();
            if (dop == null)
                return;
            CostCentre recepient = null;
            if (order.DocumentIssuerCostCentre is DistributorSalesman)
                recepient = order.DocumentIssuerCostCentre;
            else
                recepient = order.DocumentRecipientCostCentre;
            CostCentre issuer =_costCentreRepository.GetAll().OfType<DistributorPendingDispatchWarehouse>().First();
            InventoryTransferNote _transferToPhone = _inventoryTransferNoteFactory.Create(issuer,
                                                                              config.CostCentreApplicationId,
                                                                              order.DocumentIssuerUser,
                                                                              recepient,
                                                                              order.IssuedOnBehalfOf,
                                                                              order.DocumentReference);
            _transferToPhone.DocumentParentId = order.Id;
            DispatchNote dispatctFromPhone = _dispatchNoteFactory.Create(issuer, config.CostCentreApplicationId,
                                                                         recepient, order.DocumentIssuerUser,
                                                                         order.IssuedOnBehalfOf,
                                                                         DispatchNoteType.DispatchToPhone,
                                                                         order.DocumentReference, order.ParentId,
                                                                         order.Id);
           
            foreach (var item in order.PendingDispatchLineItems)
            {
                _transferToPhone.AddLineItem(_inventoryTransferNoteFactory.CreateLineItem(item.Product.Id, item.ApprovedQuantity, 0, 0, order.DocumentReference));
                dispatctFromPhone.AddLineItem(_dispatchNoteFactory.CreateLineItem(item.Product.Id,item.ApprovedQuantity,0,order.DocumentReference,0,0,item.ProductDiscount,item.DiscountType));
            }
            _transferToPhone.Confirm();
            _inventoryTransferNoteWfManager.SubmitChanges(_transferToPhone,config);

           
           
            dispatctFromPhone.Confirm();
            _dispatchNoteWfManager.SubmitChanges(dispatctFromPhone,config);
            

        }

        private void HandleApprovedCommand(MainOrder order,BasicConfig config)
        {
            
            var approvedlineItemCommands = order.GetSubOrderCommandsToExecute().OfType<ApproveOrderLineItemCommand>();
            
           if (!approvedlineItemCommands.Any())
                return;

           DistributorPendingDispatchWarehouse pendingDispatchWarehouse = _costCentreRepository.GetAll().OfType<DistributorPendingDispatchWarehouse>().First();

            CostCentre issuer = _costCentreRepository.GetById(config.CostCentreId);
             CostCentre recepient = null;
            if (order.DocumentIssuerCostCentre is DistributorSalesman)
                recepient = order.DocumentIssuerCostCentre;
            else
                recepient = order.DocumentRecipientCostCentre;
            InventoryTransferNote inventoryTransfernote = _inventoryTransferNoteFactory.Create(issuer,
                                                                              config.CostCentreApplicationId,
                                                                              order.DocumentIssuerUser,
                                                                              pendingDispatchWarehouse,
                                                                              order.IssuedOnBehalfOf,
                                                                              order.DocumentReference);
            inventoryTransfernote.DocumentParentId = inventoryTransfernote.Id;
            string invoiceRef = _getDocumentReference.GetDocReference("Inv", recepient.Id,order.IssuedOnBehalfOf.Id);
            if (order.IsEditable())
            {
                Domain.Transactional.DocumentEntities.Invoice invoice = _invoiceFactory.Create(issuer, config.CostCentreApplicationId, recepient,
                                                         order.DocumentIssuerUser, invoiceRef, order.Id, order.Id);
                foreach (var _item in order.PendingApprovalLineItems)
                {
                        invoice.AddLineItem(_invoiceFactory.CreateLineItem(_item.Product.Id, _item.Qty,
                                                                           _item.Value, order.DocumentReference, 0,
                                                                           _item.LineItemVatValue, _item.ProductDiscount,
                                                                           _item.DiscountType));
                }
                invoice.Confirm();
                _invoiceWorkFlowManager.SubmitChanges(invoice,config);
                if(order.GetPayments.Any(a=>a.IsConfirmed && !a.IsProcessed))
                {

                    Guid currentInvoice = invoice.Id;

                    string receiptRef = _getDocumentReference.GetDocReference("Rpt", recepient.Id,order.IssuedOnBehalfOf.Id);
                    Receipt receipt = _receiptFactory.Create(order.DocumentIssuerCostCentre,
                                                             config.CostCentreApplicationId,
                                                             order.DocumentRecipientCostCentre,
                                                             order.DocumentIssuerUser, receiptRef, order.Id, currentInvoice,
                                                             Guid.Empty);

                    foreach (var info in order.GetPayments.Where(s => s.IsConfirmed && !s.IsProcessed))
                    {
                        receipt.AddLineItem(_receiptFactory.CreateLineItem(info.Amount, info.PaymentRefId,
                                                                           info.MMoneyPaymentType,
                                                                           info.NotificationId, 0,
                                                                           info.PaymentModeUsed, info.Description, receipt.Id,
                                                                           info.IsConfirmed)
                            );
                    }
                    receipt.Confirm();
                    _receiptWorkFlowManager.SubmitChanges(receipt,config);
                }

            }

            foreach (var _item in approvedlineItemCommands)
            {
                ApproveOrderLineItemCommand ap = _item as ApproveOrderLineItemCommand;
                SubOrderLineItem soli = order.PendingApprovalLineItems.First(s => s.Id == ap.LineItemId);
                if (ap.ApprovedQuantity > 0)
                {
                    inventoryTransfernote.AddLineItem(_inventoryTransferNoteFactory.CreateLineItem(soli.Product.Id, ap.ApprovedQuantity, 0, 0, order.DocumentReference));
                    
                }
              //  _commandRouter.RouteDocumentCommand(_item);
            }
            inventoryTransfernote.Confirm();
            _inventoryTransferNoteWfManager.SubmitChanges(inventoryTransfernote,config);
           
        }
    }
}
