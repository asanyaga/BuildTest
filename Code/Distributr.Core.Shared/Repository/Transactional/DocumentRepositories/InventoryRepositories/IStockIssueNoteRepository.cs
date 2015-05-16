using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories
{
    public interface IStockIssueNoteRepository
    {
        List<StockIssueNote> GetByIssuerCostCentre(int issuerCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued);
        List<StockIssueNote> GetByRecipientCostCentre(int recipientCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued);
    }
}
