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
    public  class InventoryTransferNoteFactory : BaseDocumentFactory, IInventoryTransferNoteFactory
    {
        private IProductRepository _productRepository;

        public InventoryTransferNoteFactory(ICostCentreRepository costCentreRepository, IUserRepository userRepository, IProductRepository productRepository) 
            : base(costCentreRepository, userRepository)
        {
            _productRepository = productRepository;
        }


        public InventoryTransferNote Create(CostCentre documentIssuerCostCentre, Guid documentIssuerCostCentreApplicationId,
                                            User documentIssuerUser, CostCentre documentRecipientCostCentre,
                                            CostCentre documentIssuedOnBehalfOfCostCentre, string documentReference)
        {
            Guid id = Guid.NewGuid();
            InventoryTransferNote doc = DocumentPrivateConstruct<InventoryTransferNote>(id);
            doc.DocumentType = DocumentType.InventoryTransferNote;
            doc.DocumentParentId = id;
            Map(doc,documentIssuerCostCentre, documentIssuerCostCentreApplicationId,documentRecipientCostCentre,
                documentIssuerUser,documentReference,null, null);

            SetDefaultDates(doc);
            doc.EnableAddCommands();
            return doc;
        }

        public InventoryTransferNoteLineItem CreateLineItem(Guid productId, decimal qty, decimal value, int lineItemSequenceNo, string description)
        {
            Product product = _productRepository.GetById(productId);
            var li = DocumentLineItemPrivateConstruct<InventoryTransferNoteLineItem>(Guid.NewGuid());
            li.Qty = qty;
            li.Product = product;
            li.LineItemSequenceNo = lineItemSequenceNo;
            li.Value = value;
            li.Description = description;
            return li;
        }

    }
}
