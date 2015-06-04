using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Repository.Transactional.DocumentRepositories.InventoryRepositories
{
    public interface IInventoryAdjustmentNoteRepository : IDocumentRepository<InventoryAdjustmentNote>
    {
        List<InventoryAdjustmentNote> GetByIssuerCostCentre(int issuerCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued);
        List<InventoryAdjustmentNote> GetByRecipientCostCentre(int recipientCostCentreId, DocumentStatus? status, DateTime? fromDateIssued, DateTime? toDateIssued);
   
    }
}
