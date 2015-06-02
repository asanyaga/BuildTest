using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.Discounts
{
    public class ChangeDiscountLineItemCommand : AfterCreateCommand
    {
        public ChangeDiscountLineItemCommand()
        {
            
        }
        public ChangeDiscountLineItemCommand(Guid commandId, Guid documentId, 
            Guid commandGeneratedByUserId, Guid commandGeneratedByCostCentreId, 
            int costCentreApplicationCommandSequenceId, 
            Guid commandGeneratedByCostCentreApplicationId,
            Guid lineItemId, decimal newQuantity,Guid parentDocId) : base(commandId, documentId, 
            commandGeneratedByUserId, commandGeneratedByCostCentreId, 
            costCentreApplicationCommandSequenceId, 
            commandGeneratedByCostCentreApplicationId, parentDocId)
        {
            LineItemId = lineItemId;
            NewQuantity = newQuantity;
        }

        public Guid LineItemId { get; set; }
        public decimal NewQuantity { get; set; }
        public override string CommandTypeRef
        {
            get { throw new NotImplementedException("What command type is this"); }
        }
    }
}
