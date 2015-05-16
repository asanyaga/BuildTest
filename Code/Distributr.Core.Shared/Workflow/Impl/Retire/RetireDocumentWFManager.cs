using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.ClientApp;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Domain.Master.SettingsEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Master.SettingsRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DispatchRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReturnsRepositories;

namespace Distributr.Core.Workflow.Impl.Retire
{
    public class RetireDocumentWFManager : IRetireDocumentWFManager
    {
        private IOutgoingDocumentCommandRouter _commandRouter;
        private IRetireDocumentSettingRepository _retireSettingRepository;
        private IOrderRepository _orderService;
        private IInvoiceRepository _invoiceService;
        private IReceiptRepository _receiptService;
        private ICreditNoteRepository _creditNoteService;
        private IDispatchNoteRepository _dispatchNoteService;
        private IInventoryAdjustmentNoteRepository _inventoryAdjustmentNoteService;
        private IInventoryTransferNoteRepository _inventoryTransferNoteService;
        private IInventoryReceivedNoteRepository _inventoryReceivedNoteService;
        private IReturnsNoteRepository _returnsNoteService;

        public RetireDocumentWFManager(IOutgoingDocumentCommandRouter commandRouter, IRetireDocumentSettingRepository retireSettingRepository, IOrderRepository orderService, 
            IInvoiceRepository invoiceService, IReceiptRepository receiptService, ICreditNoteRepository creditNoteService, IDispatchNoteRepository dispatchNoteService, 
            IInventoryAdjustmentNoteRepository inventoryAdjustmentNoteService, IInventoryTransferNoteRepository inventoryTransferNoteService, 
            IInventoryReceivedNoteRepository inventoryReceivedNoteService, IReturnsNoteRepository returnsNoteService)
        {
            _commandRouter = commandRouter;
            _retireSettingRepository = retireSettingRepository;
            _orderService = orderService;
            _invoiceService = invoiceService;
            _receiptService = receiptService;
            _creditNoteService = creditNoteService;
            _dispatchNoteService = dispatchNoteService;
            _inventoryAdjustmentNoteService = inventoryAdjustmentNoteService;
            _inventoryTransferNoteService = inventoryTransferNoteService;
            _inventoryReceivedNoteService = inventoryReceivedNoteService;
            _returnsNoteService = returnsNoteService;
        }


        public void Submit(RetireDocumentCommand command, DocumentType documentType)
        {
            _commandRouter.RouteDocumentCommandWithOutSave(command);
            if (documentType == DocumentType.Order)
            {
                RetireDocument(command.DocumentId);
            }
            else if (documentType == DocumentType.ReturnsNote)
            {
                RemoveClosedReturnsDocument(command.DocumentId);
            }
        }
        public RetireDocumentSetting GetSetting()
        {
            return _retireSettingRepository.GetSettings();
        }

        public List<Order> GetDeliveredOrders(int duration)
        {
            DateTime filter = DateTime.Now.AddDays(-duration);
            List<Order> orders = _orderService.GetByDocumentStatus(DocumentStatus.Closed).Where(o => o.DateRequired <= filter).ToList();
            return orders;
        }

        public List<Order> GetFullPaidOrder(int duration)
        {
            DateTime filter = DateTime.Now.AddDays(-duration);
            List<Order> ordersList = _orderService.GetByDocumentStatus(DocumentStatus.Closed).Where(o => o.DateRequired <= filter).ToList();
            List<Order> orders = new List<Order>();
            var OrderPaymentInfos = LoadAllPaymentInfos(ordersList);
            var orderIds2 = OrderPaymentInfos.Where(n => n.AmountDue <= 0).Select(n => n.OrderId).ToList();
            orderIds2.Distinct().ToList().ForEach(n =>
            {
                Order o = _orderService.GetById(n) as Order;
                if (o != null)
                    orders.Add(o);
            });
            return orders;
        }

        public void RetireDocument(Guid parentId)
        {
            //_orderService.Archive(parentId);
            //_invoiceService.Archive(parentId);
            //_receiptService.Archive(parentId);
            //_creditNoteService.Archive(parentId);
            //_dispatchNoteService.Archive(parentId);
        }

        public void RemoveOldInventoryDocument()
        {
            DateTime date = DateTime.Now.AddDays(-2);
            var inventoryAdjustmentnote = _inventoryAdjustmentNoteService.GetAll().Where(d => d.StartDate < date);
            foreach (var i in inventoryAdjustmentnote)
            {
                //TODO Implement
                //_inventoryAdjustmentNoteService.Archive(i.DocumentParentId);
            }
            var itn = _inventoryTransferNoteService.GetAll().Where(d => d.StartDate < date);
            foreach (var i in itn)
            {
                //_inventoryTransferNoteService.Archive(i.DocumentParentId);
            }
            var irn = _inventoryReceivedNoteService.GetAll().Where(d => d.StartDate < date);
            foreach (var i in irn)
            {
                //_inventoryReceivedNoteService.Archive(i.DocumentParentId);
            }
        }

        public List<ReturnsNote> GetClosedReturns(int duration)
        {
            DateTime date = DateTime.Now.AddDays(-duration);
            var returns = _returnsNoteService.GetAll().OfType<ReturnsNote>().Where(d => d.StartDate < date && d.Status == DocumentStatus.Closed);
            return returns.ToList();
        }

        public void RemoveClosedReturnsDocument(Guid parentId)
        {
            //_returnsNoteService.Archive(parentId);
        }

        public List<InvoicePaymentInfo> LoadAllPaymentInfos(List<Order> orders = null, DateTime startDate = new DateTime(), DateTime endDate = new DateTime())
        {
            List<InvoicePaymentInfo> OrderPaymentInfos = new List<InvoicePaymentInfo>();
            List<Core.Domain.Transactional.DocumentEntities.Invoice> AllInvoices = new List<Core.Domain.Transactional.DocumentEntities.Invoice>();
            List<CreditNote> InvoiceCreditNotes = new List<CreditNote>();

            OrderPaymentInfos.Clear();
            AllInvoices.Clear();
            InvoiceCreditNotes.Clear();

            if (startDate.Equals(new DateTime()))
            {
                if (orders == null)
                {
                    AllInvoices = _invoiceService.GetAll().OfType<Core.Domain.Transactional.DocumentEntities.Invoice>()
                        .Where(n => n.Status != DocumentStatus.Rejected).ToList();
                }
                else
                {
                    AllInvoices = orders.Select(n => _invoiceService.GetInvoiceByOrderId(n.Id)).ToList();
                }
            }
            else
            {
                if (orders == null)
                {
                    AllInvoices = _invoiceService.GetAll(startDate, DateTime.Now).OfType<Core.Domain.Transactional.DocumentEntities.Invoice>()
                        .Where(n => n.Status != DocumentStatus.Rejected).ToList();
                }
                else
                {
                    AllInvoices = orders.Where(n => n.DocumentDateIssued >= startDate && n.DocumentDateIssued <= endDate)
                        .Select(n => _invoiceService.GetInvoiceByOrderId(n.Id)).ToList();
                }
            }

            foreach (var inv in AllInvoices)
            {
                if (inv == null)
                    continue;
                var invoiceReceipts = _receiptService.GetByInvoiceId(inv.Id);
                var receiptsTotal = new decimal();
                if (invoiceReceipts != null)
                {
                    foreach (var r in invoiceReceipts)
                    {
                        receiptsTotal = receiptsTotal + r.Total;
                    }
                }

                var invoiceCreditNotes = _creditNoteService.GetCreditNotesByInvoiceId(inv.Id);
                var creditNotesTotals = new decimal();
                if (invoiceCreditNotes != null)
                {
                    foreach (var cn in invoiceCreditNotes)
                    {
                        creditNotesTotals = creditNotesTotals + cn.Total;
                    }
                }

                OrderPaymentInfos.Add(new InvoicePaymentInfo
                {
                    InvoiceId = inv.Id,
                    OrderId = inv.OrderId,
                    InvoiceAmount = inv.TotalGross,
                    AmountPaid = receiptsTotal,
                    CreditNoteAmount = creditNotesTotals,
                    AmountDue = (inv.TotalGross - creditNotesTotals) - receiptsTotal,
                    InvoiceDate = inv.DocumentDateIssued
                });
            }
            return OrderPaymentInfos;
        }

    }
}
