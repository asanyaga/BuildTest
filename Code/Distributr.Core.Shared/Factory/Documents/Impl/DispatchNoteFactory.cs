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
    public class DispatchNoteFactory : BaseDocumentFactory, IDispatchNoteFactory
    {
        private IProductRepository _productRepository;

        public DispatchNoteFactory(ICostCentreRepository costCentreRepository, IUserRepository userRepository, IProductRepository productRepository)
            : base(costCentreRepository, userRepository)
        {
            _productRepository = productRepository;
        }

        public DispatchNote Create(CostCentre documentIssuerCostCentre, Guid documentIssueCostCentreApplicationId, CostCentre documentRecipientCC, User documentIssuerUser, CostCentre issuedOnBehalfOf, DispatchNoteType dispatchNoteType, string documentReference, Guid documentParentId, Guid orderId)
        {
            Guid id = Guid.NewGuid();
            DispatchNote doc = DocumentPrivateConstruct<DispatchNote>(id);
            doc.DocumentType = DocumentType.DispatchNote;
            doc.DispatchType = dispatchNoteType;
            doc.OrderId = orderId;


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

        public DispatchNoteLineItem CreateLineItem(Guid productId, decimal quantity, decimal value, string description, int lineItemSuquenceNo, decimal lineItemVatValue, decimal productDiscount, DiscountType discountType)
        {
            Product product = _productRepository.GetById(productId);
            var li = DocumentLineItemPrivateConstruct<DispatchNoteLineItem>(Guid.NewGuid());
            li.Product = product;
            li.Qty = (value < 0 ? -quantity : quantity);
            li.Value = value;
            li.Description = description;
            li.LineItemSequenceNo = lineItemSuquenceNo;
            li.LineItemVatValue = lineItemVatValue;
            //li.LineItemType = lineItemType;
            li.ProductDiscount = productDiscount;
            li.DiscountType = discountType;

            return li;
        }
    }
}
