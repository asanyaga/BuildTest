using System;

namespace Distributr.Core.Commands.DocumentCommands.Orders
{   [Obsolete]
    public class ConfirmOrderCommand : ConfirmCommand
    {
        public ConfirmOrderCommand()
        {
            
        }
        public ConfirmOrderCommand(Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,Guid parentDocId)
            : base(commandId, documentId, commandGeneratedByUserId, 
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId, 
            commandGeneratedByCostCentreApplicationId,parentDocId
            )

        {

        }
        public override string CommandTypeRef
        {
            get { return CommandType.ConfirmOrder.ToString(); }
        }
    }
    public class ConfirmMainOrderCommand : ConfirmCommand
    {
        public ConfirmMainOrderCommand()
        {

        }
        public ConfirmMainOrderCommand(Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId, Guid parentDocId)
            : base(commandId, documentId, commandGeneratedByUserId,
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId, parentDocId
            )
        {

        }
        public override string CommandTypeRef
        {
            get { return CommandType.ConfirmMainOrder.ToString(); }
        }
    }
}
