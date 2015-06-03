using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.Receipts
{
    public class ConfirmReceiptLineItemCommand : AfterCreateCommand
    {
        public ConfirmReceiptLineItemCommand()
        {
            
        }
       public ConfirmReceiptLineItemCommand(
           Guid commandId, 
           Guid documentId, 
           Guid lineItemId,
           Guid commandGeneratedByUserId, 
           Guid commandGeneratedByCostCentreId, 
           int costCentreApplicationCommandSequenceId, 
           Guid commandGeneratedByCostCentreApplicationId,
           string paymentRefId,
           string description, Guid parentDocId
           ) 
           : base(commandId, documentId, commandGeneratedByUserId, commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId, commandGeneratedByCostCentreApplicationId, parentDocId)
       {
           LineItemId = lineItemId;
           PaymentRefId = paymentRefId;
           Description = description;
       }
       public Guid LineItemId { get; set; }
       public string PaymentRefId { get; set; }
       public override string CommandTypeRef
       {
           get { return CommandType.ConfirmReceiptLineItem.ToString(); }
       }
    }
}
