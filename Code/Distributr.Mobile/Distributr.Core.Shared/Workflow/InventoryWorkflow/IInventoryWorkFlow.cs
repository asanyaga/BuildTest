using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Workflow.InventoryWorkflow
{
    public interface IInventoryWorkflow
    {
        void InventoryAdjust(Guid costCentreId, Guid productId, decimal qty, 
            DocumentType docType, Guid documentId, DateTime date, InventoryAdjustmentNoteType inventoryAdjustmentNoteType);
    }
    public interface ISourcingInventoryWorkflow
    {
        void InventoryAdjust(Guid costCentreId, Guid commodityId,Guid gradeId, decimal qty);
    }
}
