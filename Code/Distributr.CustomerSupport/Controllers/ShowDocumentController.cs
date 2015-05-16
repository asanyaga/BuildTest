using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DisbursementRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DispatchRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.OrderRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.PaymentNoteRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReturnsRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Newtonsoft.Json;

namespace Distributr.CustomerSupport.Controllers
{
    public class ShowDocumentController : ApiController
    {
        private IGenericDocumentRepository _genericDocumentRepository;
        private IOrderRepository _orderRepository;
        private IDispatchNoteRepository _dispatchNoteRepository;
        private IInventoryReceivedNoteRepository _inventoryReceivedNoteRepository;
        private IInvoiceRepository _invoiceRepository;
        private IReturnsNoteRepository _returnsNoteRepository;
        private IReceiptRepository _receiptRepository;
        private IInventoryAdjustmentNoteRepository _inventoryAdjustmentNoteRepository;
        private ICreditNoteRepository _creditNoteRepository;
        private IDisbursementNoteRepository _disbursementNoteRepository;
        private IPaymentNoteRepository _paymentNoteRepository;
        private ICommodityPurchaseRepository _commodityPurchaseRepository;
        private IInventoryTransferNoteRepository _inventoryTransferNoteRepository;

        public ShowDocumentController(IGenericDocumentRepository genericDocumentRepository, IOrderRepository orderRepository, IDispatchNoteRepository dispatchNoteRepository, IInventoryReceivedNoteRepository inventoryReceivedNoteRepository, IInvoiceRepository invoiceRepository, IReturnsNoteRepository returnsNoteRepository, IReceiptRepository receiptRepository, IInventoryAdjustmentNoteRepository inventoryAdjustmentNoteRepository, ICreditNoteRepository creditNoteRepository, IDisbursementNoteRepository disbursementNoteRepository, IPaymentNoteRepository paymentNoteRepository, ICommodityPurchaseRepository commodityPurchaseRepository, IInventoryTransferNoteRepository inventoryTransferNoteRepository)
        {
            _genericDocumentRepository = genericDocumentRepository;
            _orderRepository = orderRepository;
            _dispatchNoteRepository = dispatchNoteRepository;
            _inventoryReceivedNoteRepository = inventoryReceivedNoteRepository;
            _invoiceRepository = invoiceRepository;
            _returnsNoteRepository = returnsNoteRepository;
            _receiptRepository = receiptRepository;
            _inventoryAdjustmentNoteRepository = inventoryAdjustmentNoteRepository;
            _creditNoteRepository = creditNoteRepository;
            _disbursementNoteRepository = disbursementNoteRepository;
            _paymentNoteRepository = paymentNoteRepository;
            _commodityPurchaseRepository = commodityPurchaseRepository;
            _inventoryTransferNoteRepository = inventoryTransferNoteRepository;
        }

        [HttpGet]
        public HttpResponseMessage Get(Guid documentId)
        {
            HttpStatusCode returnCode = HttpStatusCode.OK;

            Document doc = GetDocument(documentId);
            string _doc = JsonConvert.SerializeObject(doc);
            return Request.CreateResponse(returnCode,
                new
                {
                    Title = "My Title",
                    Document = _doc
                }
                );
        }

        private Document GetDocument(Guid documentId)
        {
            Document doc = null;
            DocumentType documentType = _genericDocumentRepository.GetById(documentId).DocumentType;
            switch (documentType)
            {
                case DocumentType.Order :
                    doc = _orderRepository.GetById(documentId);
                    break;
                case DocumentType.DispatchNote :
                    doc = _dispatchNoteRepository.GetById(documentId);
                    break;
                case DocumentType.InventoryReceivedNote :
                    doc = _inventoryReceivedNoteRepository.GetById(documentId);
                    break;
                case DocumentType.InventoryTransferNote :
                    doc = _inventoryTransferNoteRepository.GetById(documentId);
                    break;
                case DocumentType.Invoice :
                    doc = _invoiceRepository.GetById(documentId);
                    break;
                case DocumentType.SalesInvoice :
                    throw new Exception("Does not have repository");
                    break;
                case DocumentType.ReturnsNote :
                    doc = _returnsNoteRepository.GetById(documentId);
                    break;
                case DocumentType.Receipt :
                    doc = _receiptRepository.GetById(documentId);
                    break;
                case DocumentType.InventoryAdjustmentNote :
                    doc = _inventoryAdjustmentNoteRepository.GetById(documentId);
                    break;
                case DocumentType.CreditNote :
                    doc = _creditNoteRepository.GetById(documentId);
                    break;
                case DocumentType.DisbursementNote :
                    doc = _disbursementNoteRepository.GetById(documentId);
                    break;
                case DocumentType.PaymentNote :
                    doc = _paymentNoteRepository.GetById(documentId);
                    break;
                case DocumentType.CommodityPurchaseNote:
                case DocumentType.CommodityReceptionNote:
                case DocumentType.CommodityStorageNote:
                    //doc = _commodityPurchaseRepository.GetById(documentId);
                    break;
            }


            return doc;
        }

    }
}
