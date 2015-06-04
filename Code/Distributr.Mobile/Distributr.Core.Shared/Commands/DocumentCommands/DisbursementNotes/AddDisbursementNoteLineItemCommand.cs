using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.DisbursementNotes
{
    public class AddDisbursementNoteLineItemCommand : AfterCreateCommand
    {
        public AddDisbursementNoteLineItemCommand()
        {
            
        }
       public AddDisbursementNoteLineItemCommand(
           Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            int lineItemSequenceNo,
            Guid productId,
            decimal qty,
             decimal lineItemValue, Guid parentDocId
            )
            : base(
            commandId,
            documentId, 
            commandGeneratedByUserId,
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId, parentDocId
            )
        {
            LineItemSequenceNo = lineItemSequenceNo;
            ProductId = productId;
            Qty = qty;
            Value = lineItemValue;
        }

        public int LineItemSequenceNo { get;  set; }

        public Guid ProductId { get;  set; }
        public decimal Qty { get;  set; }
        public decimal Value { get;  set; }
        public override string CommandTypeRef
        {
            get { return CommandType.AddDisbursementNoteLineItem.ToString(); }
        }
    }
}
