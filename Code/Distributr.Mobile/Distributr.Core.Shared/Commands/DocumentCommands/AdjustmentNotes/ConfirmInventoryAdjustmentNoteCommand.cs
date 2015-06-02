using System;


namespace Distributr.Core.Commands.DocumentCommands.AdjustmentNotes
{
    public class ConfirmInventoryAdjustmentNoteCommand : ConfirmCommand
    {
        public ConfirmInventoryAdjustmentNoteCommand()
        {
            
        }
        public ConfirmInventoryAdjustmentNoteCommand(Guid commandId,
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
            get { return CommandType.ConfirmInventoryAdjustmentNote.ToString(); }
        }
    }
}
