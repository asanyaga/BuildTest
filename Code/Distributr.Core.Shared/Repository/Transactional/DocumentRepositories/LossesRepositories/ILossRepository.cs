using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Repository.Transactional.DocumentRepositories.LossesRepositories
{
    public interface ILossRepository : IDocumentRepository<PaymentNote>, IDocumentRepositorySaveable<PaymentNote>
    {
        List<PaymentNote> GetByIssuerCostCentre(int issuerCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued);
        List<PaymentNote> GetByRecipientCostCentre(int recipientCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued);

    }
}
