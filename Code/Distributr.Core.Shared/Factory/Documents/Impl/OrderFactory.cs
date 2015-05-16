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
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;

namespace Distributr.Core.Factory.Documents.Impl
{
    public class OrderFactory : BaseDocumentFactory, IOrderFactory
    {
        private IProductRepository _productRepository;
        public OrderFactory(ICostCentreRepository costCentreRepository, IUserRepository userRepository, IProductRepository productRepository) : base(costCentreRepository, userRepository)
        {
            _productRepository = productRepository;
        }

        public Order Create(CostCentre documentIssuerCostCentre, Guid documentIssueCostCentreApplicationId, CostCentre documentRecipientCC, User documentIssuerUser, CostCentre issuedOnBehalfOf, OrderType orderType, string documentReference, Guid documentParentId, DateTime dateRequired)
        {
            Guid id = Guid.NewGuid();
            Order doc = DocumentPrivateConstruct<Order>(id);
            doc.DocumentType = DocumentType.Order;
            doc.OrderType = orderType;
            doc.IssuedOnBehalfOf = issuedOnBehalfOf;
            doc.DateRequired = dateRequired;

            if (documentParentId == null || documentParentId == Guid.Empty)
                doc.DocumentParentId = id;
            else
                doc.DocumentParentId = documentParentId;
            Map(doc, documentIssuerCostCentre, documentIssueCostCentreApplicationId, documentRecipientCC,
                documentIssuerUser, documentReference, null, null);
            SetDefaultDates(doc);
            doc.EnableAddCommands();
            return doc;
        }

        public OrderLineItem CreateLineItem(Guid productId, decimal quantity, decimal value, string description, int lineItemSuquenceNo, decimal lineItemVatValue, OrderLineItemType lineItemType, decimal productDiscount, DiscountType discountType)
        {
            Product product = _productRepository.GetById(productId);
            var li = DocumentLineItemPrivateConstruct<OrderLineItem>(Guid.NewGuid());
            li.Product = product;
            li.Qty = quantity;
            li.Value = value;
            li.Description = description;
            li.LineItemSequenceNo = lineItemSuquenceNo;
            li.LineItemVatValue = lineItemVatValue;
            li.LineItemType = lineItemType;
            li.ProductDiscount = productDiscount;
            li.DiscountType = discountType;

            return li;
        }
    }
}
