using System;

namespace Distributr.Core.Commands.DocumentCommands.Orders
{
    public class CloseOrderCommand : CloseCommand
    {
        public CloseOrderCommand()
        {
            
        }
        public CloseOrderCommand(Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId)
            : base(commandId, documentId, commandGeneratedByUserId,
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId,documentId
            )
        {

        }

        public override string CommandTypeRef
        {
            get { return CommandType.CloseOrder.ToString(); }
        }
    }
}
