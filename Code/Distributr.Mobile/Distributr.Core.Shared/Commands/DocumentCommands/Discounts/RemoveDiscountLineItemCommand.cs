using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.Discounts
{
    public class RemoveDiscountLineItemCommand : AfterCreateCommand
    {
        public RemoveDiscountLineItemCommand()
        {
            
        }
        public RemoveDiscountLineItemCommand(Guid commandId, Guid documentId, 
            Guid commandGeneratedByUserId, Guid commandGeneratedByCostCentreId, 
            int costCentreApplicationCommandSequenceId, 
            Guid commandGeneratedByCostCentreApplicationId,
            Guid lineItemId, Guid parentDocId)
            : base(commandId, documentId, commandGeneratedByUserId, 
            commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId, 
            commandGeneratedByCostCentreApplicationId, parentDocId)
        {
            LineItemId = lineItemId;
        }

        public Guid LineItemId { get;  set; }
        public override string CommandTypeRef
        {
            get { throw new NotImplementedException("What command type is this?"); }
        }
    }
}
