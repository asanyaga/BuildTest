using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;

namespace Distributr.Core.Factory.Documents
{
    public interface IOrderFactory
    {
        Order Create(CostCentre documentIssuerCostCentre, Guid documentIssueCostCentreApplicationId, CostCentre documentRecipientCC,
                                       User documentIssuerUser, CostCentre issuedOnBehalfOf, OrderType orderType, string documentReference, Guid documentParentId, DateTime dateRequired);

        OrderLineItem CreateLineItem(Guid productId, decimal quantity, decimal value, string description,
                                     int lineItemSuquenceNo, decimal lineItemVatValue, OrderLineItemType lineItemType,
                                     decimal productDiscount, DiscountType discountType);
    }
    public interface IMainOrderFactory
    {
        MainOrder Create(CostCentre documentIssuerCostCentre, Guid documentIssueCostCentreApplicationId, CostCentre documentRecipientCC,
                                       User documentIssuerUser, CostCentre issuedOnBehalfOf, OrderType orderType, string documentReference, Guid documentParentId,string shipToAddress, DateTime dateRequired, decimal saleDiscount,string note="");

        SubOrderLineItem CreateLineItem(Guid productId, decimal quantity, decimal value, string description
                                     , decimal lineItemVatValue
                                     );

        SubOrderLineItem CreateLineItem(Product product, decimal quantity, decimal value, string description
                             , decimal lineItemVatValue
                             );

        SubOrderLineItem CreateDiscountedLineItem(Guid productId, decimal quantity, decimal value, string description, decimal lineItemVatValue,
                                    decimal productDiscount);
        SubOrderLineItem CreateFOCLineItem(Guid productId, decimal quantity, string description, DiscountType discountType);
    }
}
