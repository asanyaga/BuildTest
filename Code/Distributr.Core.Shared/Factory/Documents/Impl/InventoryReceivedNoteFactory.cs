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
    public class InventoryReceivedNoteFactory : BaseDocumentFactory, IInventoryReceivedNoteFactory
    {
        private IProductRepository _productRepository;
        public InventoryReceivedNoteFactory(ICostCentreRepository costCentreRepository, IUserRepository userRepository, IProductRepository productRepository)
            :base(costCentreRepository, userRepository)
        {
            _productRepository = productRepository;
        }
        public InventoryReceivedNote Create(CostCentre documentIssuerCostCentre, Guid documentIssueCostCentreApplicationId,
                                            CostCentre documentRecipientCC, CostCentre goodsReceivedFromCostCentre, string loadNo,
                                            string orderReference, User documentIssuerUser, string documentReference,
                                            Guid documentParentId)
        {
            Guid id = Guid.NewGuid();
            InventoryReceivedNote doc = DocumentPrivateConstruct<InventoryReceivedNote>(id);
            doc.DocumentType = DocumentType.InventoryReceivedNote;
            if (documentParentId == null || documentParentId == Guid.Empty)
                doc.DocumentParentId = id;
            else
                doc.DocumentParentId = documentParentId;
            Map(doc,documentIssuerCostCentre,documentIssueCostCentreApplicationId,documentRecipientCC,documentIssuerUser,documentReference,null,null);
            SetDefaultDates(doc);
            doc.EnableAddCommands();
            doc.GoodsReceivedFromCostCentre = goodsReceivedFromCostCentre;
            doc.LoadNo = loadNo;
            doc.OrderReferences = orderReference;
            return doc;
        }

        public InventoryReceivedNoteLineItem CreateLineItem(Guid productId, decimal quantity, 
                                                            decimal value, string description,
                                                            int lineItemSequenceNo)
        {
            Product product = _productRepository.GetById(productId);
            var li = DocumentLineItemPrivateConstruct<InventoryReceivedNoteLineItem>(Guid.NewGuid());
            li.Product = product;
            li.Description = description;
            li.Qty = quantity;
            li.Value = value;
            li.LineItemSequenceNo = lineItemSequenceNo;
            return li;
        }
    }
}
