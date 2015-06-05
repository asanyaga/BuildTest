using System;
using System.Linq;
using Distributr.Core.Commands.DocumentCommands.AdjustmentNotes;
using Distributr.Core.Commands.DocumentCommands.DispatchNotes;
using Distributr.Core.Commands.DocumentCommands.InventoryTransferNotes;
using Distributr.Core.Commands.DocumentCommands.Invoices;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Commands.DocumentCommands.Receipts;
using Distributr.Core.Domain.Master.ProductEntities;
using Distributr.Core.Domain.Transactional;
using Distributr.Mobile.Core.OrderSale;
using Distributr.Mobile.Core.Outlets;
using Distributr.Mobile.Core.Products;
using Distributr.Mobile.Core.Test;
using Order = Distributr.Mobile.Core.OrderSale.Order;

namespace Distributr.Mobile.Core.Sync.Incoming
{    
    public class IncomingCommandHandler
    {
        private readonly ISaleRepository saleRepository;
        private readonly IOutletRepository outletRepository;
        private readonly ISaleProductRepository saleProductRepository;
        private readonly IInventoryRepository inventoryRepository;
        private readonly IReturnableProductRepository returnableProductRepository;

        private Sale currentSale;
        private Guid? currentParentGuid;
        private int currentDispatchNoteType;

        public IncomingCommandHandler(ISaleRepository saleRepository, IOutletRepository outletRepository, ISaleProductRepository saleProductRepository, IInventoryRepository inventoryRepository, IReturnableProductRepository returnableProductRepository)
        {
            this.saleRepository = saleRepository;
            this.outletRepository = outletRepository;
            this.saleProductRepository = saleProductRepository;
            this.inventoryRepository = inventoryRepository;
            this.returnableProductRepository = returnableProductRepository;
        }

        public void Handle(CreateMainOrderCommand command)
        {
            var order = GetOrder();
            if (order != null)
            {
                // We ignore the CreateMainOrderCommand when the order already exists. 
                // The Hub sends a CreateMainOrderCommand when it can not fulfil the complete order (i.e. back order)
                // See remark on AddMainOrderLineItemCommand also.
                return;
            }

            currentSale = new Sale(command.PDCommandId, outletRepository.GetById(command.IssuedOnBehalfOfCostCentreId))
            {
                ShipToAddress = command.ShipToAddress,
                SaleDiscount = command.SaleDiscount,
                Note = command.Note,
                OrderReference = command.DocumentReference,
                ProcessingStatus = ProcessingStatus.Submitted,
                _DateCreated = command.DocumentDateIssued,
            };
        }

        public void Handle(ApproveOrderLineItemCommand command)
        {
            var order = GetOrder();
            var approvedItem = order.AllItems.FirstOrDefault(i => i.Id == command.LineItemId);
            
            if (approvedItem != null)
            {
                approvedItem.LineItemStatus = LineItemStatus.Approved;
                if (approvedItem is ReturnableLineItem) return;
                approvedItem.SaleQuantity = command.ApprovedQuantity;
            }
            else
            {
                order.AllItems.ForEach(i => Console.WriteLine(i.Id));
                //Can't handle this yet without a change to the Hub
                Console.WriteLine("Ignoring line item {0} - {1}", order.OrderReference, command.LineItemId);
            }
        }

        public void Handle(AddMainOrderLineItemCommand command)
        {
            var order = GetOrder();
            var item = order.AllItems.FirstOrDefault(i => i.ProductMasterId == command.ProductId);
            if (item == null)
            {
                var product = saleProductRepository.FindById(command.ProductId);
                
                //Product will be null for returnable items 
                if (product == null )
                {
                    item = AddReturnableLineItem(order, command);
                }
                else
                {
                    item = AddProductLineItem(order, product, command);
                }

                item.Quantity = command.Qty;
            }
        }

        private BaseProductLineItem AddProductLineItem(Order order, SaleProduct product, AddMainOrderLineItemCommand command)
        {
            var vatRate = command.LineItemVatValue/command.ValueLineItem;

            return order.AddItem(command.CommandId, product, product.Id, command.Qty, command.ValueLineItem, vatRate);
        }

        private BaseProductLineItem AddReturnableLineItem(Order order, AddMainOrderLineItemCommand command)
        {
            var returnableProduct = returnableProductRepository.GetById(command.ProductId);
            var rerturnableItem = order.AddReturnableItem(command.CommandId, returnableProduct, command.ProductId, command.Qty, command.ValueLineItem);
            return rerturnableItem;
        }

        public void Handle(AddReceiptLineItemCommand command)
        {
            var order = GetOrder();
            if (order == null) return;

            var existing = order.Payments.Find(p => p.PaymentReference == command.Description);
            if (existing != null) return;

            var payment = new Payment(order.Id)
            {
                PaymentStatus = PaymentStatus.Confirmed,
                PaymentMode = (PaymentMode) command.LineItemType,
                PaymentReference = command.Description,
                Amount = command.Value
            };
            order.Payments.Add(payment);
        }

        public void Handle(ApproveMainOrderCommand command)
        {
            var order = GetOrder();
            order.ProcessingStatus = ProcessingStatus.Approved;
        }

        public void Handle(CreateInvoiceCommand command)
        {
            var order = GetOrder();
            if (order != null)
            {
                order.InvoiceReference = command.DocumentReference;
                order.InvoiceId = command.DocumentId;
            }
        }

        public void Handle(OrderDispatchApprovedLineItemsCommand command)
        {
            var order = GetOrder();
            order.ProcessingStatus = ProcessingStatus.Deliverable;
        }

        public void Handle(CloseOrderCommand command)
        {
            var order = GetOrder();
            if (order != null)
            {
                order.ProcessingStatus = ProcessingStatus.Confirmed;
            }
        }

        public void Handle(CreateDispatchNoteCommand command)
        {
            currentDispatchNoteType = command.DispatchNoteType;
        }

        public void Handle(AddDispatchNoteLineItemCommand command)
        {
            if (currentDispatchNoteType != 2) return;
            
            var order = GetOrder();
            if (order != null)
            {
                var item = order.ReturnableLineItems.Find(r => r.ProductMasterId == command.ProductId && r.Quantity == command.Qty);
                if (item == null) return;
                item.SaleQuantity = command.Qty;
                item.LineItemStatus = LineItemStatus.Approved;
            }
        }

        public void Handle(AddInventoryTransferNoteLineItemCommand command)
        {
            // Only adjust inventory when it is a standalone inventory adjustment that is not
            // the result of a dispatched order. Order's manage their own inventory and use the approved
            // quantity as their inventory. The inventory adjusted here is what is available for point-of-sale stuff. 
            if (command.PDCommandId == currentParentGuid.GetValueOrDefault())
            {
                inventoryRepository.AdjustInventoryForProduct(command.ProductId, command.Qty);
            }            
        }

        public void Handle(AddInventoryAdjustmentNoteLineItemCommand command)
        {
            inventoryRepository.AdjustInventoryForProduct(command.ProductId, command.Actual - command.Expected);
        }

        public void Handle(ChangeMainOrderLineItemCommand command)
        {
            var order = GetOrder();
            var item = order.AllItems.Find(l => l.Id == command.LineItemId);
            if (item != null)
            {
                item.Quantity = command.NewQuantity;
            }
        }

        public void Handle(RemoveMainOrderLineItemCommand command)
        {
            var order = GetOrder();
            order.LineItems.RemoveAll(l => l.Id == command.LineItemId);
        }

        public void Handle(RejectMainOrderCommand command)
        {
            var order = GetOrder();
            if (order != null)
            {
                order.ProcessingStatus = ProcessingStatus.Rejected;
            }
        }

        //Default handler for commands that don't impact the system
        public void Handle(object ignored)
        {
            //Console.WriteLine("Ignoring {0}", JsonConvert.SerializeObject(ignored, Formatting.Indented));
        }

        public Order GetOrder()
        {
            return currentSale ?? (currentSale = saleRepository.FindById(currentParentGuid.GetValueOrDefault()));
        }

        public void Save()
        {
            if (currentSale != null)
            {
                saleRepository.Save(currentSale);
            }
        }

        public void Init(Guid parentDoucmentGuid)
        {
            currentParentGuid = parentDoucmentGuid;
            currentDispatchNoteType = -1;
            currentSale = null;
        }
    }
}
