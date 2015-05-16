using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.CreditNotes
{
    public class ConfirmCreditNoteCommand : ConfirmCommand
    {
        public ConfirmCreditNoteCommand()
        {
            
        }
        public ConfirmCreditNoteCommand(
            //Command
                Guid commandId,
                Guid documentId,
                Guid commandGeneratedByUserId,
                Guid commandGeneratedByCostCentreId,
                int costCentreApplicationCommandSequenceId,
                Guid commandGeneratedByCostCentreApplicationId, Guid parentDocId)
            : base(commandId, documentId, commandGeneratedByUserId, commandGeneratedByCostCentreId,
                costCentreApplicationCommandSequenceId, commandGeneratedByCostCentreApplicationId, parentDocId)
        {

        }

        public override string CommandTypeRef
        {
            get { return CommandType.ConfirmCreditNote.ToString(); }
        }
    }
}
