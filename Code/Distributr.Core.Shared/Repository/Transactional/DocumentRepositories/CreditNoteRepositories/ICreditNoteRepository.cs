using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories
{
    public interface ICreditNoteRepository : IDocumentRepository<CreditNote>, IDocumentRepositorySaveable<CreditNote>
    {
        List<CreditNote> GetCreditNotesByInvoiceId(Guid invoiceId = new Guid());
    }
}
