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
    public class CreditNoteFactory : BaseDocumentFactory, ICreditNoteFactory
    {
        private IProductRepository _productRepository;

        public CreditNoteFactory( IProductRepository productRepository) 
        {
            _productRepository = productRepository;
        }

        public CreditNote Create(CostCentre documentIssuerCostCentre, Guid documentIssueCostCentreApplicationId, CostCentre documentRecipientCC, User documentIssuerUser, string documentReference, Guid documentParentId, Guid invoiceId)
        {
            Guid id = Guid.NewGuid();
            CreditNote doc = DocumentPrivateConstruct<CreditNote>(id);
            doc.DocumentType = DocumentType.CreditNote;
            doc.InvoiceId = invoiceId;
            if (documentParentId == null || documentParentId == Guid.Empty)
                doc.DocumentParentId = id;
            else
                doc.DocumentParentId = documentParentId;
            Map(doc, documentIssuerCostCentre, documentIssueCostCentreApplicationId, documentRecipientCC, documentIssuerUser, documentReference, null, null);
            SetDefaultDates(doc);
            doc.EnableAddCommands();
            return doc;
        }

        public CreditNoteLineItem CreateLineItem(Guid productId, decimal quantity, decimal value, string description, int lineItemSequenceNo, decimal lineItemVatValue, decimal productDiscount)
        {
            Product product = _productRepository.GetById(productId);
            var li = DocumentLineItemPrivateConstruct<CreditNoteLineItem>(Guid.NewGuid());
            li.Product = product;
            li.Description = description;
            li.Qty = quantity;
            li.Value = value;
            li.LineItemSequenceNo = lineItemSequenceNo;
            li.LineItemVatValue = lineItemVatValue;
            li.ProductDiscount = productDiscount;
           return li;
        }
    }
}
