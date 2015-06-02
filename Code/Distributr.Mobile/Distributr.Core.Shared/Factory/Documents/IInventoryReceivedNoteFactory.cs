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
    public interface IInventoryReceivedNoteFactory
    {
        InventoryReceivedNote Create(CostCentre documentIssuerCostCentre, Guid documentIssueCostCentreApplicationId, CostCentre documentRecipientCC,
                                        CostCentre goodsReceivedFromCostCentre, string loadNo, string orderReference,
                                       User DocumentIssuerUser, string DocumentReference, 
                                       Guid documentParentId);
        InventoryReceivedNoteLineItem CreateLineItem(Guid productId, decimal quantity, decimal value, 
            string description, int lineItemSequenceNo);

    }
}
