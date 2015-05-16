using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.ReturnsNotes
{
    public class CreateReturnsNoteCommand : CreateCommand
    {
        public CreateReturnsNoteCommand()
        {
            
        }
        public CreateReturnsNoteCommand(
            //Command
            Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            //Returns Note
            string documentReference,
            DateTime dateReturnsNoteCreated,
            Guid documentIssuerCostCentreId,
            Guid documentRecipientCostCentreId,
            Guid documentIssuerUserId,
            int returnsNoteTypeId,
            DateTime documentDateIssued
            )
            : base(commandId, documentId, commandGeneratedByUserId,
                commandGeneratedByCostCentreId,
                costCentreApplicationCommandSequenceId,
                commandGeneratedByCostCentreApplicationId,documentId,
            documentDateIssued,
            documentIssuerCostCentreId,documentIssuerUserId,
            documentReference
            )
        {
            DateReturnsNoteCreated = dateReturnsNoteCreated;
            DocumentIssuerCostCentreId = documentIssuerCostCentreId;
            DocumentRecipientCostCentreId = documentRecipientCostCentreId;
            ReturnsNoteTypeId = returnsNoteTypeId;
        }

        public DateTime DateReturnsNoteCreated { get; set; }
        public Guid DocumentRecipientCostCentreId { get; set; }
        public int ReturnsNoteTypeId { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.CreateReturnsNote.ToString(); }
        }
    }
}
