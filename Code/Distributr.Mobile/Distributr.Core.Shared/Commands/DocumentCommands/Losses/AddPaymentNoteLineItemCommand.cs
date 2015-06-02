using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.Losses
{
    public class AddPaymentNoteLineItemCommand : AfterCreateCommand
    {
        public AddPaymentNoteLineItemCommand()
        {
            
        }
        public AddPaymentNoteLineItemCommand(
           Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            int lineItemSequenceNo,
            decimal amount,
            int paymentModeId
            )
            : base(
            commandId,
            documentId, 
            commandGeneratedByUserId,
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId, documentId
            )
        {
            LineItemSequenceNo = lineItemSequenceNo;
            Amount = amount;
            PaymentModeId = paymentModeId;
        }
        public int LineItemSequenceNo { get; set; }
        public int PaymentModeId { get; set; }
        public decimal Amount { get; set; }

        public override string CommandTypeRef
        {
            get { return CommandType.AddPaymentNoteLineItem.ToString(); }
        }       
    }
}
