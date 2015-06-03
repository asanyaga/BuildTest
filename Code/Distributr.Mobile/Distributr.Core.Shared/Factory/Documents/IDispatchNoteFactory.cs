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
    public interface IDispatchNoteFactory
    {
        DispatchNote Create(CostCentre documentIssuerCostCentre, Guid documentIssueCostCentreApplicationId, CostCentre documentRecipientCC,
                                      User documentIssuerUser, CostCentre issuedOnBehalfOf, DispatchNoteType dispatchNoteType, string documentReference, Guid documentParentId, Guid orderId);

        DispatchNoteLineItem CreateLineItem(Guid productId, decimal quantity, decimal value, string description,
                                     int lineItemSuquenceNo, decimal lineItemVatValue,
                                     decimal productDiscount, DiscountType discountType);
    }
}
