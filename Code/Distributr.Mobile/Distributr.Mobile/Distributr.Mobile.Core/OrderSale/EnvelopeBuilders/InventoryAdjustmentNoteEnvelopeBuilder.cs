using System;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.AdjustmentNotes;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.Products;

namespace Distributr.Mobile.Core.OrderSale.EnvelopeBuilders
{
    public class InventoryAdjustmentNoteEnvelopeBuilder : BaseEnvelopeBuilder
    {
        private readonly IInventoryRepository inventoryRepository;

        public InventoryAdjustmentNoteEnvelopeBuilder(IEnvelopeContext context, IInventoryRepository inventoryRepository)
            : base(context)
        {
            this.inventoryRepository = inventoryRepository;
        }

        public InventoryAdjustmentNoteEnvelopeBuilder(IEnvelopeContext context, IInventoryRepository inventoryRepository, IEnvelopeBuilder linkedBuilder) : base(context, linkedBuilder)
        {
            this.inventoryRepository = inventoryRepository;
        }

        protected override void CreateEnvelope()
        {
            Envelope = InitEnvelope();
            Envelope.DocumentTypeId = (int) DocumentType.InventoryAdjustmentNote;
        }

        protected override DocumentCommand CreateFirstCommand()
        {
            var command = InitCommand(new CreateInventoryAdjustmentNoteCommand());
            command.InventoryAdjustmentNoteTypeId = Context.InventoryAdjustmentNoteType;
            command.DocumentRecipientCostCentreId = Context.RecipientCostCentreId;
            return command;
        }

        protected override void AddConfirmCommand()
        {
            Commands.Add(InitCommand(new ConfirmInventoryAdjustmentNoteCommand()));
        }

        protected override void ProcessLineItem(BaseProductLineItem item, decimal quantity)
        {
            var returnableItem = item as ReturnableProductLineItem;

            var adjustmentReason = "Order Inventory";

            if (returnableItem != null)
            {
                if (returnableItem.SaleQuantity < 1) return;

                adjustmentReason = "Returnable Inventory";
                quantity = returnableItem.SaleQuantity;
            }

            var command = InitCommand(new AddInventoryAdjustmentNoteLineItemCommand());

            var available = inventoryRepository.GetBalanceForProduct(item.ProductMasterId);

            command.Actual = available - quantity;
            command.Expected = available;
            command.Reason = adjustmentReason;
            command.Description = adjustmentReason;
            command.ProductId = item.ProductMasterId;
            Commands.Add(command);
        }

        protected override void ProcessPayment(Payment payment)
        {
            //Not used by InventoryAdjustmentNote
        }
    }
}
