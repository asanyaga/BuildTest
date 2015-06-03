using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.StockIssueNotes
{
    public class ConfirmStockIssueNoteCommand: DocumentCommand
    {
        public ConfirmStockIssueNoteCommand(Guid commandId,
            Guid documentId,
            int commandGeneratedByUserId,
            int commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            int commandGeneratedByCostCentreApplicationId)
            : base(commandId, documentId, commandGeneratedByUserId, 
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId, 
            commandGeneratedByCostCentreApplicationId
            )
        {

        }
    }
}
