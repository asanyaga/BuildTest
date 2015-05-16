using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.Losses
{
    public class ConfirmPaymentNoteCommand : ConfirmCommand
    {
        public ConfirmPaymentNoteCommand()
        {
            
        }
        public ConfirmPaymentNoteCommand(
            Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId)
            : base(
            commandId,
            documentId,
            commandGeneratedByUserId,
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId, documentId
            )
        {

        }

        public override string CommandTypeRef
        {
            get { return CommandType.ConfirmPaymentNote.ToString(); }
        }
    }
}
