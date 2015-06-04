using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;

namespace Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories
{
    public interface IReceiptRepository : IDocumentRepository<Receipt>, IDocumentRepositorySaveable<Receipt>
    {
        List<Receipt> GetReceipts(Guid invoiceId = new Guid());
        Receipt GetByLineItemId(Guid itemId);
        List<ReceiptLineItem> GetChildLineItemsByLineItemId(Guid parentLineItemId);
        List<Receipt> GetByInvoiceId(Guid invoiceId);
    }
}
