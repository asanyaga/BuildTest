using System;

namespace Distributr.Core.Commands.DocumentCommands.AdjustmentNotes
{
    public class AddInventoryAdjustmentNoteLineItemCommand : AfterCreateCommand
    {
        public AddInventoryAdjustmentNoteLineItemCommand()
        {
            
        }
       public AddInventoryAdjustmentNoteLineItemCommand(
           Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            Guid productId,
            decimal actual,
            decimal expected,
           int lineItemSequence, string reason
           )
           : base(commandId, documentId, commandGeneratedByUserId, commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId,
commandGeneratedByCostCentreApplicationId, documentId)
       {

           ProductId = productId;
           Actual = actual;
           Expected = expected;
           LineItemSequence = lineItemSequence;
           Reason = reason;
       }



       public Guid ProductId { get;  set; }
       public decimal Actual { get;  set; }
        public decimal Expected { get;  set; }
        public int LineItemSequence { get;  set; }
        public string Reason { get;  set; }
        public override string CommandTypeRef
        {
            get { return CommandType.AddInventoryAdjustmentNoteLineItem.ToString(); }
        }
    }
}
