using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.CreditNotes
{
    public class CreateCreditNoteCommand : CreateCommand
    {
        public CreateCreditNoteCommand()
        {
            
        }
        public CreateCreditNoteCommand(
            //Command
            Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            //credit note
            string documentReference,
            DateTime dateCreditNoteCreated,
            Guid documentRecipientCostCentreId,
            
            Guid invoiceId,
            int creditNoteType, Guid parentDocId
            )
            : base(commandId, documentId, commandGeneratedByUserId,
                commandGeneratedByCostCentreId,
                costCentreApplicationCommandSequenceId,
                commandGeneratedByCostCentreApplicationId, parentDocId,
            dateCreditNoteCreated, commandGeneratedByCostCentreId,
            commandGeneratedByUserId, documentReference)
        {
            DocumentReference = documentReference;
           DocumentRecipientCostCentreId = documentRecipientCostCentreId;
            InvoiceId = invoiceId;
            CreditNoteType = creditNoteType;
        }

        public Guid DocumentRecipientCostCentreId { get; set; }
        public Guid InvoiceId { get; set; } //cn
        public int CreditNoteType { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.CreateCreditNote.ToString(); }
        }
    }
}
