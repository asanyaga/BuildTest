using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Repository.Transactional.DocumentRepositories.DispatchRepositories
{
    public interface IDispatchNoteRepository : IDocumentRepository<DispatchNote>,  IDocumentRepositorySaveable<DispatchNote>
    {
        List<DispatchNote> GetByIssuerCostCentre(Guid issuerCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued);
        List<DispatchNote> GetByRecipientCostCentre(Guid recipientCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued);

        List<DispatchNote> GetByOrderId(Guid orderId);
    }
}
