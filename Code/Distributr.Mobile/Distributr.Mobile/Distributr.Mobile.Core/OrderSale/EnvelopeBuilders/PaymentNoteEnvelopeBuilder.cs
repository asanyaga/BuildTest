using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.Losses;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.Envelopes;

namespace Distributr.Mobile.Core.OrderSale.EnvelopeBuilders
{
    public class PaymentNoteEnvelopeBuilder : BaseEnvelopeBuilder
    {
        public PaymentNoteEnvelopeBuilder(IEnvelopeContext context) : base(context)
        {
        }

        public PaymentNoteEnvelopeBuilder(IEnvelopeContext context, IEnvelopeBuilder linkedBuilder) : base(context, linkedBuilder)
        {
        }

        protected override void CreateEnvelope()
        {
            Envelope = InitEnvelope();
            Envelope.DocumentTypeId = (int) DocumentType.PaymentNote;
        }

        protected override DocumentCommand CreateFirstCommand()
        {
            var command = InitCommand(new CreatePaymentNoteCommand());
            command.PaymentNoteTypeId = (int) PaymentNoteType.Availabe;
            command.PaymentNoteRecipientCostCentreId = Context.RecipientCostCentreId;

            return command;
        }

        protected override void AddConfirmCommand()
        {
            Commands.Add(InitCommand(new ConfirmPaymentNoteCommand()));
        }

        protected override void ProcessLineItem(BaseProductLineItem item, decimal quantity)
        {
            //Not used for PaymentNote
        }

        protected override void ProcessPayment(Payment payment)
        {
            var command = InitCommand(new AddPaymentNoteLineItemCommand());
            command.Amount = payment.Amount;
            command.PaymentModeId = 0; //Always zero (copied from existing app)
            command.LineItemSequenceNo = Commands.Count + 1;

            Commands.Add(command);
        }
    }
}
