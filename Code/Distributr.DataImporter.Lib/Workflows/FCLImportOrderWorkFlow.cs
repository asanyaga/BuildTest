using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Distributr.Core.Workflow;
using Distributr.WPF.Lib.Services.Service.Transactional.Commands.CommandRouting;
using Distributr.WPF.Lib.Services.Service.Utility;
using Distributr.WPF.Lib.Services.WorkFlow.GetDocumentReferences;

namespace Distributr.DataImporter.Lib.Workflows
{

    public class FCLImportOrderWorkFlow : IExternalOrderWorkflow
    {
        private IOutgoingDocumentCommandRouter _commandRouter;
        private InventoryTransferNoteFactory _inventoryTransferNoteFactory;
        private IConfigService _configService;
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
        public FCLImportOrderWorkFlow(IInvoiceRepository invoiceRepository, IOutgoingDocumentCommandRouter commandRouter, InventoryTransferNoteFactory inventoryTransferNoteFactory, IConfigService configService, IConfirmInventoryTransferNoteWFManager inventoryTransferNoteWfManager, ICostCentreRepository costCentreRepository, IDispatchNoteFactory dispatchNoteFactory, IConfirmDispatchNoteWFManager dispatchNoteWfManager, IInvoiceFactory invoiceFactory, IConfirmInvoiceWorkFlowManager invoiceWorkFlowManager, IGetDocumentReference getDocumentReference, IReceiptWorkFlowManager receiptWorkFlowManager, IReceiptFactory receiptFactory, ICreditNoteFactory creditNoteFactory, IConfirmCreditNoteWFManager confirmCreditNoteWfManager)
        {
            _commandRouter = commandRouter;
            _inventoryTransferNoteFactory = inventoryTransferNoteFactory;
            _configService = configService;
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
            _invoiceRepository = invoiceRepository;
        }

        public void Submit(MainOrder order)
        {
            List<DocumentCommand> commandsToExecute = order.GetSubOrderCommandsToExecute();
            var createCommand = commandsToExecute.OfType<CreateCommand>().FirstOrDefault();
            if (createCommand != null)
                _commandRouter.RouteDocumentCommand(createCommand);
            var lineItemCommands = commandsToExecute.OfType<AfterCreateCommand>();
            foreach (var _item in lineItemCommands)
            {
                _commandRouter.RouteDocumentCommand(_item);
            }
            var editlineItemCommands = commandsToExecute.OfType<ChangeMainOrderLineItemCommand>();
            foreach (var _editeditem in editlineItemCommands)
            {
                _commandRouter.RouteDocumentCommand(_editeditem);
            }
            var removedLineitem = commandsToExecute.OfType<RemoveMainOrderLineItemCommand>();
            foreach (var _removeditem in removedLineitem)
            {
                _commandRouter.RouteDocumentCommand(_removeditem);
            }
            var co = commandsToExecute.OfType<ConfirmCommand>().FirstOrDefault();
            if (co != null)
                _commandRouter.RouteDocumentCommand(co);

            //transfer inventory to pending warehouse
            HandleApprovedCommand(order);
            var aop = commandsToExecute.OfType<ApproveCommand>().FirstOrDefault();
            if (aop != null)
                _commandRouter.RouteDocumentCommand(aop);

            // transfer inventory to pending warehouse
            HandleDispatchLineItemsCommand(order);
            HandlePayments(order);
            var closecommand = commandsToExecute.OfType<CloseOrderCommand>().FirstOrDefault();
            if (closecommand != null)
                _commandRouter.RouteDocumentCommand(closecommand);
            var rco = commandsToExecute.OfType<RejectMainOrderCommand>().FirstOrDefault();
            if (rco != null)
            {
                _commandRouter.RouteDocumentCommand(rco);
                HandleLostSale(order);
            }
        }

        private void HandleLostSale(MainOrder order)
        {
            if (!order.IsEditable())
            {
                Invoice invoice = _invoiceRepository.GetInvoiceByOrderId(order.Id);
                string creditnoteRef = _getDocumentReference.GetDocReference("CN", order.DocumentReference);
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
                _confirmCreditNoteWFManager.SubmitChanges(cn);
            }
        }

        private void HandlePayments(MainOrder order)
        {
            Config config = _configService.Load();
            var orderpaymentInfoitem = order.GetSubOrderCommandsToExecute().OfType<AddOrderPaymentInfoCommand>();
            foreach (var paymentInfo in orderpaymentInfoitem)
            {
                _commandRouter.RouteDocumentCommand(paymentInfo);
            }
            var invoice = _invoiceRepository.GetInvoiceByOrderId(order.Id);
            if (orderpaymentInfoitem.Any(s => s.IsConfirmed && !s.IsProcessed) && invoice != null)
            {
                Guid currentInvoice = invoice.Id;

                string receiptRef = _getDocumentReference.GetDocReference("Rpt", order.DocumentReference);
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
                _receiptWorkFlowManager.SubmitChanges(receipt);

            }


        }

        private void HandleDispatchLineItemsCommand(MainOrder order)
        {
            var dop = order.GetSubOrderCommandsToExecute().OfType<OrderDispatchApprovedLineItemsCommand>().FirstOrDefault();
            if (dop == null)
                return;
            Config config = _configService.Load();
            CostCentre recepient = null;
            if (order.DocumentIssuerCostCentre is DistributorSalesman)
                recepient = order.DocumentIssuerCostCentre;
            else
                recepient = order.DocumentRecipientCostCentre;
            CostCentre issuer = _costCentreRepository.GetAll().OfType<DistributorPendingDispatchWarehouse>().First();
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
                dispatctFromPhone.AddLineItem(_dispatchNoteFactory.CreateLineItem(item.Product.Id, item.ApprovedQuantity, 0, order.DocumentReference, 0, 0, item.ProductDiscount, item.DiscountType));
            }
            _transferToPhone.Confirm();
            _inventoryTransferNoteWfManager.SubmitChanges(_transferToPhone);


            _commandRouter.RouteDocumentCommand(dop);
            dispatctFromPhone.Confirm();
            _dispatchNoteWfManager.SubmitChanges(dispatctFromPhone);


        }

        private void HandleApprovedCommand(MainOrder order)
        {
            Config config = _configService.Load();
            var approvedlineItemCommands = order.GetSubOrderCommandsToExecute().OfType<ApproveOrderLineItemCommand>();

            if (!approvedlineItemCommands.Any())
                return;
            CostCentre issuer = null;
            CostCentre recepient = null;
            if (order.DocumentIssuerCostCentre is Distributor)
            {
                recepient = order.DocumentIssuerCostCentre;
                issuer = order.DocumentRecipientCostCentre;
            }
           
            else
            {
                recepient = order.DocumentRecipientCostCentre;
                issuer = order.DocumentIssuerCostCentre;
            }
            var getAll = _costCentreRepository.GetAll().OfType<DistributorPendingDispatchWarehouse>();
            DistributorPendingDispatchWarehouse pendingDispatchWarehouse =
                getAll.FirstOrDefault(p => p.ParentCostCentre.Id == recepient.Id);
           
            InventoryTransferNote inventoryTransfernote = _inventoryTransferNoteFactory.Create(recepient,
                                                                              config.CostCentreApplicationId,
                                                                              order.DocumentIssuerUser,
                                                                              pendingDispatchWarehouse,
                                                                              order.IssuedOnBehalfOf,
                                                                              order.DocumentReference);
            inventoryTransfernote.DocumentParentId = inventoryTransfernote.Id;
            string invoiceRef = _getDocumentReference.GetDocReference("Inv", order.DocumentReference);
            if (order.IsEditable())
            {
                Invoice invoice = _invoiceFactory.Create(issuer, config.CostCentreApplicationId, recepient,
                                                         order.DocumentIssuerUser, invoiceRef, order.Id, order.Id);
                foreach (var _item in order.PendingApprovalLineItems)
                {
                    invoice.AddLineItem(_invoiceFactory.CreateLineItem(_item.Product.Id, _item.Qty,
                                                                       _item.Value, order.DocumentReference, 0,
                                                                       _item.LineItemVatValue, _item.ProductDiscount,
                                                                       _item.DiscountType));
                }
                invoice.Confirm();
                _invoiceWorkFlowManager.SubmitChanges(invoice);
                if (order.GetPayments.Any(a => a.IsConfirmed && !a.IsProcessed))
                {

                    Guid currentInvoice = invoice.Id;

                    string receiptRef = _getDocumentReference.GetDocReference("Rpt", order.DocumentReference);
                    Receipt receipt = _receiptFactory.Create(issuer,
                                                             config.CostCentreApplicationId,
                                                             recepient,
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
                    _receiptWorkFlowManager.SubmitChanges(receipt);
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
                _commandRouter.RouteDocumentCommand(_item);
            }
            inventoryTransfernote.Confirm();
            _inventoryTransferNoteWfManager.SubmitChanges(inventoryTransfernote);

        }
    }
}
