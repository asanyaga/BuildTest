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
using Distributr.Core.Domain.Transactional.DocumentEntities.OrderDocumentEntities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;

namespace Distributr.Core.Factory.Documents.Impl
{
    public class MainOrderFactory : BaseDocumentFactory, IMainOrderFactory
    {
        private IProductRepository _productRepository;
        public MainOrderFactory( IProductRepository productRepository)
            
        {
            _productRepository = productRepository;
        }

        public MainOrder Create(CostCentre documentIssuerCostCentre, Guid documentIssueCostCentreApplicationId, CostCentre documentRecipientCC, User documentIssuerUser, CostCentre issuedOnBehalfOf, OrderType orderType, string documentReference, Guid documentParentId,string shipToAddress, DateTime dateRequired,decimal saleDiscount,string note="")
        {
            Guid id = Guid.NewGuid();
            MainOrder doc = DocumentPrivateConstruct<MainOrder>(id);
            doc.DocumentType = DocumentType.Order;
            doc.OrderType = orderType;
            doc.IssuedOnBehalfOf = issuedOnBehalfOf;
            doc.DateRequired = dateRequired;
            doc.OrderStatus = OrderStatus.New;
            doc.DocumentParentId = id;
            doc.ParentId = id;
            doc.SaleDiscount = saleDiscount;
            doc.ShipToAddress = shipToAddress;
            doc.Note = note;
         
            Map(doc, documentIssuerCostCentre, documentIssueCostCentreApplicationId, documentRecipientCC,
                documentIssuerUser, documentReference, null, null);
            SetDefaultDates(doc);
            doc.EnableAddCommands();
            SubOrder subOrder = MakeSubOrder(doc);
            AddSubDocument(subOrder, doc);

            return doc;
        }

        void AddSubDocument(SubOrder subOrder, MainOrder order)
        {
            
            MethodInfo info = typeof (MainOrder)
                .GetMethod("AddSubOrder", BindingFlags.NonPublic | BindingFlags.Instance);
            info.Invoke(order, new[] {subOrder});
        }
        private SubOrder MakeSubOrder(MainOrder doc)
        {
            SubOrder subOrder = DocumentPrivateConstruct<SubOrder>(doc.Id);
             Map(subOrder,doc.DocumentIssuerCostCentre, doc.DocumentIssuerCostCentreApplicationId, doc.DocumentRecipientCostCentre,
                doc.DocumentIssuerUser, doc.DocumentReference, null, null);
            subOrder.DocumentType = doc.DocumentType;
            subOrder.OrderType = doc.OrderType;
            subOrder.IssuedOnBehalfOf = doc.IssuedOnBehalfOf;
            subOrder.DateRequired = doc.DateRequired;
            subOrder.DocumentParentId = doc.Id;
            subOrder.ParentId = doc.Id;
            subOrder.OrderStatus = doc.OrderStatus;
            subOrder.SaleDiscount = doc.SaleDiscount;
            subOrder.ShipToAddress = doc.ShipToAddress;
            subOrder.Note = doc.Note;
             SetDefaultDates(subOrder);
             subOrder.EnableAddCommands();
            return subOrder;
        }

        public SubOrderLineItem CreateLineItem(Guid productId, decimal quantity, decimal value, string description
                                     , decimal lineItemVatValue
                                     )
        {
            Product product = _productRepository.GetById(productId);
            return CreateLineItem(product, quantity, value, description, lineItemVatValue);
        }

        public SubOrderLineItem CreateLineItem(Product product, decimal quantity, decimal value, string description
                                        , decimal lineItemVatValue
                                        )
        {
            var li = DocumentLineItemPrivateConstruct<SubOrderLineItem>(Guid.NewGuid());
            li.Product = product;
            li.Qty = quantity;
            li.Value = value;
            li.Description = description;
            li.LineItemSequenceNo = 0;
            li.LineItemVatValue = lineItemVatValue;
            li.LineItemType = MainOrderLineItemType.Sale;
            li.DiscountType = DiscountType.None;
            li.LineItemStatus = MainOrderLineItemStatus.New;

            return li;
        }

        public SubOrderLineItem CreateDiscountedLineItem(Guid productId, decimal quantity, decimal value, string description,
                                                         decimal lineItemVatValue, decimal productDiscount)
        {
            Product product = _productRepository.GetById(productId);
            var li = DocumentLineItemPrivateConstruct<SubOrderLineItem>(Guid.NewGuid());
            li.Product = product;
            li.Qty = quantity;
            li.Value = value;
            li.Description = description;
            li.LineItemSequenceNo = 0;
            li.ProductDiscount = productDiscount;
            li.LineItemVatValue = lineItemVatValue;
            li.LineItemType = MainOrderLineItemType.Sale;
            li.DiscountType = DiscountType.ProductDiscount;
            li.LineItemStatus = MainOrderLineItemStatus.New;

            return li;
        }

        public SubOrderLineItem CreateFOCLineItem(Guid productId, decimal quantity, string description,DiscountType discountType)
        {
            Product product = _productRepository.GetById(productId);
            var li = DocumentLineItemPrivateConstruct<SubOrderLineItem>(Guid.NewGuid());
            li.Product = product;
            li.Qty = quantity;
            li.Value = 0;
            li.Description = description;
            li.LineItemSequenceNo = 0;
            li.ProductDiscount = 0;
            li.LineItemVatValue = 0;
            li.LineItemType = MainOrderLineItemType.Discount;
            li.DiscountType = discountType;
            li.LineItemStatus = MainOrderLineItemStatus.New;

            return li;
        }
    }
}
