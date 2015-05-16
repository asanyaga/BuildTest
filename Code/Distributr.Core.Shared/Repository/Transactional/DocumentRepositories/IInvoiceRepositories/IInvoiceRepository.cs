using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;

namespace Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories
{
    public interface IInvoiceRepository : IDocumentRepository<Invoice> 
    {
        Invoice GetInvoiceByOrderId(Guid orderId);
        //REFACTOR - Take out
        void SaveLineItem(InvoiceLineItem ili, Guid invoiceId);
        //REFACTOR - Take out
        bool ChangeStatus(Guid documentId, DocumentStatus status);
        InvoiceLineItem GetLineItemById(Guid lineItemId);
        //REFACTOR - Take out
        void RemoveLineItem(InvoiceLineItem ili);

        List<Invoice> InvoicesPendingPayment(out List<InvoicePaymentInfo> invoicePaymentInfoList);
        bool IsPendingPayment(Guid invoiceId, out InvoicePaymentInfo invoicePaymentInfo, Invoice invoice = null);
    }
}
