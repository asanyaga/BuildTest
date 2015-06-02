using System;

namespace Distributr.Core.Commands.DocumentCommands.InventoryReceivedNotes
{
    public class AddInventoryReceivedNoteLineItemCommand : AfterCreateCommand
    {
        public AddInventoryReceivedNoteLineItemCommand()
        {
            
        }
        public AddInventoryReceivedNoteLineItemCommand(
           Guid commandId,
            Guid documentId,
            Guid lineItemId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            int lineItemSequenceNo,
            Guid productId,
            decimal qty, decimal expected, decimal value)
            : base(commandId, documentId, commandGeneratedByUserId,
                commandGeneratedByCostCentreId,
                costCentreApplicationCommandSequenceId,
                commandGeneratedByCostCentreApplicationId,documentId
            )
        {
            LineItemId = lineItemId;
            LineItemSequenceNo = lineItemSequenceNo;
            ProductId = productId;
            Qty = qty;
            Expected = expected;
            Value = value;
        }
        public Guid LineItemId { get; set; }

        public int LineItemSequenceNo { get; set; }

        public Guid ProductId { get; set; }
        public decimal Qty { get; set; }
        public decimal Expected { get; set; }
        public decimal Value { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.AddInventoryReceivedNoteLineItem.ToString(); }
        }

    }
}
