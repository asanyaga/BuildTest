using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.DisbursementNotes
{
    public class CreateDisbursementNoteCommand : CreateCommand
    {
        public CreateDisbursementNoteCommand()
        {
            
        }
         public CreateDisbursementNoteCommand(
            Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            Guid documentIssuerCostCentreId,
            Guid documentRecipientCostCentreId,
            Guid documentIssuerUserId, Guid parentDocId,
             DateTime documentDateIssued,
             string documentReference
            )
            : base(commandId, documentId, commandGeneratedByUserId,
                commandGeneratedByCostCentreId,
                costCentreApplicationCommandSequenceId,
                commandGeneratedByCostCentreApplicationId, 
             parentDocId,
             documentDateIssued,
             documentIssuerCostCentreId,
             documentIssuerUserId, 
             documentReference)
        {
            DocumentRecipientCostCentreId = documentRecipientCostCentreId;
        }


         public Guid DocumentRecipientCostCentreId { get; set; }
         public override string CommandTypeRef
         {
             get { return CommandType.CreateDisbursementNote.ToString(); }
         }
    }
    
}
