using System;

namespace Distributr.Core.Commands.DocumentCommands.DispatchNotes
{
    public class ConfirmDispatchNoteCommand : ConfirmCommand
    {
        public ConfirmDispatchNoteCommand()
        {
            
        }
        public ConfirmDispatchNoteCommand(
            Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId, Guid parentDocId)
            : base(
            commandId,
            documentId,
            commandGeneratedByUserId, 
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId, 
            commandGeneratedByCostCentreApplicationId, parentDocId
            )

        {

        }

        public override string CommandTypeRef
        {
            get { return CommandType.ConfirmDispatchNote.ToString(); }
        }
    }
}
