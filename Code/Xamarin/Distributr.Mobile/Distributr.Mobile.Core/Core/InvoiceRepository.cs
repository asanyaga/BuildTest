
using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Mobile.Core.Documents;
using Distributr.Mobile.Data;

namespace Distributr.Mobile.Core
{
    public class InvoiceRepository : DocumentRepository<Invoice>, IInvoiceRepository
    {
        private readonly Database db;

        public InvoiceRepository(Database db)
        {
            this.db = db;
        }

        public Invoice GetInvoiceByOrderId(Guid orderId)
        {
            return default(Invoice);
        }

        public void SaveLineItem(InvoiceLineItem ili, Guid invoiceId)
        {
            throw new NotImplementedException();
        }

        public bool ChangeStatus(Guid documentId, DocumentStatus status)
        {
            throw new NotImplementedException();
        }

        public InvoiceLineItem GetLineItemById(Guid lineItemId)
        {
            throw new NotImplementedException();
        }

        public void RemoveLineItem(InvoiceLineItem ili)
        {
            throw new NotImplementedException();
        }

        public List<Invoice> InvoicesPendingPayment(out List<InvoicePaymentInfo> invoicePaymentInfoList)
        {
            throw new NotImplementedException();
        }

        public bool IsPendingPayment(Guid invoiceId, out InvoicePaymentInfo invoicePaymentInfo, Invoice invoice = null)
        {
            throw new NotImplementedException();
        }
    }
}