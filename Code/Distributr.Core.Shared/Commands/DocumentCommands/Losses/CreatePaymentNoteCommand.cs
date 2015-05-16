using System;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Commands.DocumentCommands.Losses
{
    public class CreatePaymentNoteCommand : CreateCommand
    {
        public CreatePaymentNoteCommand()
        {
            
        }
        public CreatePaymentNoteCommand(
           Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,//-- 
            Guid paymentNoteIssuerCostCentreId,
            Guid paymentNoteRecipientCostCentreId, int paymentNoteTypeId, 
            DateTime paymentNoteDateIssued,
            string documentReference
            )
            : base(commandId, documentId, commandGeneratedByUserId,
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId, documentId,
            paymentNoteDateIssued,paymentNoteIssuerCostCentreId,commandGeneratedByUserId,
            documentReference
            )
        {
          //  PaymentNoteIssuerCostCentreId = paymentNoteIssuerCostCentreId;
            PaymentNoteRecipientCostCentreId = paymentNoteRecipientCostCentreId;
          PaymentNoteTypeId = paymentNoteTypeId;
          //  PaymentNoteDateIssued = paymentNoteDateIssued;

        }

        //public Guid PaymentNoteIssuerCostCentreId { get; set; }
        public Guid PaymentNoteRecipientCostCentreId { get; set; }
     
        public int PaymentNoteTypeId { get; set; }
        //public DateTime PaymentNoteDateIssued { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.CreatePaymentNote.ToString(); }
        }
    }
}
