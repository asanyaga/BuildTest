using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.Receipts
{
    public class CreateReceiptCommand : CreateCommand
    {
        public CreateReceiptCommand()
        {
            
        }
        public CreateReceiptCommand(
            //Command
            Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,

            //receipt
              string documentReference,//
            DateTime dateReceiptCreated,//
            Guid documentIssuerCostCentreId,
            Guid documentRecipientCostCentreId,
            Guid documentIssuerUserId,
            Guid invoiceId,Guid parentDocId,
            Guid paymentDocId = new Guid()
            )
            : base(commandId, documentId, commandGeneratedByUserId,
                commandGeneratedByCostCentreId,
                costCentreApplicationCommandSequenceId,
                commandGeneratedByCostCentreApplicationId, parentDocId,
            dateReceiptCreated,documentIssuerCostCentreId,
            documentIssuerUserId, documentReference
            )
        {
            DocumentReference = documentReference;
            DocumentIssuerCostCentreId = documentIssuerCostCentreId;
            DocumentRecipientCostCentreId = documentRecipientCostCentreId;
            InvoiceId = invoiceId;
            PaymentDocId = paymentDocId;
        }

        public Guid DocumentRecipientCostCentreId { get; set; }
        public Guid InvoiceId { get; set; }
        public Guid PaymentDocId { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.CreateReceipt.ToString(); }
        }
    }
}
