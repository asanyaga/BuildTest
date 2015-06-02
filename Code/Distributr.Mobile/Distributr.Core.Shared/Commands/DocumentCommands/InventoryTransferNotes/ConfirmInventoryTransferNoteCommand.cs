using System;

namespace Distributr.Core.Commands.DocumentCommands.InventoryTransferNotes
{
    public class ConfirmInventoryTransferNoteCommand : ConfirmCommand
    {
        public ConfirmInventoryTransferNoteCommand()
        {
            
        }
        public ConfirmInventoryTransferNoteCommand(Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId)
            : base(commandId, documentId, commandGeneratedByUserId, 
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId, 
            commandGeneratedByCostCentreApplicationId, documentId
            )

        {

        }
        public override string CommandTypeRef
        {
            get { return CommandType.ConfirmInventoryTransferNote.ToString(); }
        }
    }
}
