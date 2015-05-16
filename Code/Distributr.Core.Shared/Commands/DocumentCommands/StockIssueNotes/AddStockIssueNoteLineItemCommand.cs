using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.StockIssueNotes
{
    public class AddStockIssueNoteLineItemCommand: DocumentCommand
    {
        public AddStockIssueNoteLineItemCommand(
           Guid commandId,
            Guid documentId,
            int commandGeneratedByUserId,
            int commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            int commandGeneratedByCostCentreApplicationId,
            int productId,
            int quantity,
           int lineItemSequence
           )
           : base(commandId, documentId, commandGeneratedByUserId, commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId,
commandGeneratedByCostCentreApplicationId)
       {
           ProductId = productId;
           Quantity = quantity;
           LineItemSequence = lineItemSequence;
       }     

        public int ProductId { get; private set; }
        public int Quantity { get; private set; }
        public int LineItemSequence { get; private set; }
    }
}
