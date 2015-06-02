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
    public interface IInventoryTransferNoteFactory
    {
        InventoryTransferNote Create(CostCentre documentIssuerCostCentre, Guid documentIssuerCostCentreApplicationId,
                                     User documentIssuerUser, CostCentre documentRecipientCostCentre,
                                     CostCentre documentIssuedOnBehalfOfCostCentre, string documentReference);

        InventoryTransferNoteLineItem CreateLineItem(Guid productId, decimal qty, decimal value, int lineItemSequenceNo,
                                                     string description);
    }
}
