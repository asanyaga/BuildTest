using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.DisbursementNotes
{
    public class ConfirmDisbursementNoteCommand : ConfirmCommand
    {
        public ConfirmDisbursementNoteCommand()
        {
            
        }
        public ConfirmDisbursementNoteCommand(
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
            get { return CommandType.ConfirmDisbursementNote.ToString(); }
        }
    }
}
