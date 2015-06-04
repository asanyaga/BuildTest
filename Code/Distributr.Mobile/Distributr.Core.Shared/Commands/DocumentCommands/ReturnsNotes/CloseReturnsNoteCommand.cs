using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.ReturnsNotes
{
   public class CloseReturnsNoteCommand: DocumentCommand
    {
       public CloseReturnsNoteCommand()
       {
           
       }
       public CloseReturnsNoteCommand(Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId, Guid parentDocumentId)
            : base(commandId, documentId, commandGeneratedByUserId,
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId, parentDocumentId
            )
        {

        }

       public override string CommandTypeRef
       {
           get { return CommandType.CloseReturnsNote.ToString(); }
       }
    }
}
