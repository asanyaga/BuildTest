using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Master.UserEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Factory.Master.Impl;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;

namespace Distributr.Core.Factory.Documents
{
    public class InventoryAdjustmentNoteFactory : BaseDocumentFactory, IInventoryAdjustmentNoteFactory
    {
        private IProductRepository _productRepository;
        public InventoryAdjustmentNoteFactory(ICostCentreRepository costCentreRepository, IUserRepository userRepository, IProductRepository productRepository
                                              )
            : base(costCentreRepository, userRepository)
        {
            _productRepository = productRepository;
        }

        public InventoryAdjustmentNote Create(CostCentre documentIssuerCostCentre, Guid documentIssueCostCentreApplicationId, 
            CostCentre documentRecipientCC, User documentIssuerUser,
            string documentReference, InventoryAdjustmentNoteType inventoryAdjustmentNoteType, Guid documentParentId  , double? longitude = null, double? latitude = null)
        {
            Guid id = Guid.NewGuid();
            InventoryAdjustmentNote doc = DocumentPrivateConstruct<InventoryAdjustmentNote>(id); // new InventoryAdjustmentNote(id);
            doc.InventoryAdjustmentNoteType = inventoryAdjustmentNoteType;
            doc.DocumentType = DocumentType.InventoryAdjustmentNote;
            if (documentParentId == null || documentParentId == Guid.Empty)
                doc.DocumentParentId = id;
            else
                doc.DocumentParentId = documentParentId;
            Map(doc, documentIssuerCostCentre, documentIssueCostCentreApplicationId, documentRecipientCC, documentIssuerUser, documentReference, latitude, longitude);
            SetDefaultDates(doc);
            doc.EnableAddCommands();
            return doc;
        }

        public InventoryAdjustmentNoteLineItem CreateLineItem(decimal actualQuantity, Guid productId, decimal expectedQuantity,
                                                              decimal value, string description)
        {
            Product product = _productRepository.GetById(productId);
            var li = DocumentLineItemPrivateConstruct<InventoryAdjustmentNoteLineItem>(Guid.NewGuid());
            li.Actual = actualQuantity;
            li.Description = description;
            li.LineItemSequenceNo = 0;
            li.Product = product;
            li.Qty = expectedQuantity;
            li.Value = value;

            return li;
        }

       
    }
}
