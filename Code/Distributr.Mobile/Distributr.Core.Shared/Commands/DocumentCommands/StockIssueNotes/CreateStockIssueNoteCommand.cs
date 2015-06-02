using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.StockIssueNotes
{
    public class CreateStockIssueNoteCommand: DocumentCommand
    {
        public CreateStockIssueNoteCommand(
            Guid commandId,
            Guid documentId,
            int commandGeneratedByUserId,
            int commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            int commandGeneratedByCostCentreApplicationId,
            int documentIssuerCostCentreId, 
            int docIssuerUserId
            )
            : base(commandId, documentId, commandGeneratedByUserId,
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId)
        {
            DocumentIssuerCostCentreId = documentIssuerCostCentreId;
            DocIssuerUserId = docIssuerUserId;
        }

        public int DocumentIssuerCostCentreId { get; set; }
        public int DocIssuerUserId { get; set; }
    }
}
