using System;

namespace Distributr.Core.Commands.DocumentCommands.InventoryReceivedNotes
{
    public class ConfirmInventoryReceivedNoteCommand : ConfirmCommand
    {
        public ConfirmInventoryReceivedNoteCommand()
        {
            
        }
        public ConfirmInventoryReceivedNoteCommand(Guid commandId,
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
            get { return CommandType.ConfirmInventoryReceivedNote.ToString(); }
        }
    }
}
