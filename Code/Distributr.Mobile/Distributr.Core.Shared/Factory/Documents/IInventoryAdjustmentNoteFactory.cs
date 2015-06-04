using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;

namespace Distributr.Core.Factory.Documents
{
    public interface IInventoryAdjustmentNoteFactory
    {
        InventoryAdjustmentNote Create(CostCentre documentIssuerCostCentre, Guid documentIssueCostCentreApplicationId, CostCentre documentRecipientCC,
                                       User DocumentIssuerUser, string DocumentReference, InventoryAdjustmentNoteType inventoryAdjustmentNoteType,
                                       Guid documentParentId, 
                                        double? longitude = null, double? latitude = null);

        InventoryAdjustmentNoteLineItem CreateLineItem(decimal actualQuantity, Guid productId, decimal expectedQuantity,
                                                       decimal value, string description);
    }
}
