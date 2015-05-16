using System;

namespace Distributr.Core.Commands.DocumentCommands.Orders
{
    /// <summary>
    /// Can only be removed in order confirmed status
    /// </summary>
    [Obsolete]
    public class RemoveOrderLineItemCommand : AfterCreateCommand
    {
        public RemoveOrderLineItemCommand()
        {
            
        }
        public RemoveOrderLineItemCommand(
            Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            Guid lineItemId)
            : base(commandId, documentId, commandGeneratedByUserId,
                    commandGeneratedByCostCentreId,
                    costCentreApplicationCommandSequenceId,
                    commandGeneratedByCostCentreApplicationId,documentId)
        {
            LineItemId = lineItemId;
        }

        public Guid LineItemId { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.RemoveOrderLineItem.ToString(); }
        }
    }
    public class RemoveMainOrderLineItemCommand : AfterCreateCommand
    {
        public RemoveMainOrderLineItemCommand()
        {

        }
        public RemoveMainOrderLineItemCommand(
            Guid commandId,
            Guid documentId,
            Guid lineItemId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId
            )
            : base(commandId, documentId, commandGeneratedByUserId,
                    commandGeneratedByCostCentreId,
                    costCentreApplicationCommandSequenceId,
                    commandGeneratedByCostCentreApplicationId, documentId)
        {
            LineItemId = lineItemId;
        }

        public Guid LineItemId { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.RemoveMainOrderLineItem.ToString(); }
        }
    }
}
