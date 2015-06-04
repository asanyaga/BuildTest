using System;

namespace Distributr.Core.Commands.DocumentCommands.InventoryTransferNotes
{
    public class AddInventoryTransferNoteLineItemCommand : AfterCreateCommand
    {
        public AddInventoryTransferNoteLineItemCommand()
        {
            
        }
        public AddInventoryTransferNoteLineItemCommand(
           Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            int lineItemSequenceNo,
            Guid productId,
            decimal qty)
            : base(commandId, documentId, commandGeneratedByUserId,
                    commandGeneratedByCostCentreId,
                    costCentreApplicationCommandSequenceId,
                    commandGeneratedByCostCentreApplicationId, documentId)
        {
            LineItemSequenceNo = lineItemSequenceNo;
            ProductId = productId;
            Qty = qty;
        }

        public int LineItemSequenceNo { get; set; }

        public Guid ProductId { get; set; }
        public decimal Qty { get; set; }

        public override string CommandTypeRef
        {
            get { return CommandType.AddInventoryTransferNoteLineItem.ToString(); }
        }
    }
}
