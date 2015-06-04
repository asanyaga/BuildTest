using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Commands.DocumentCommands.OutletDocument;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Mobile.Core.Envelopes;
using Distributr.Mobile.Core.OrderSale;

namespace Distributr.Mobile.Core.Outlets
{
    public class OutletVisitNoteEnvelopeBuilder : BaseEnvelopeBuilder
    {
        public OutletVisitNoteEnvelopeBuilder(IEnvelopeContext context) : base(context)
        {
            DocumentId = Context.VisitId;
        }

        public OutletVisitNoteEnvelopeBuilder(IEnvelopeContext context, IEnvelopeBuilder linkedBuilder) : base(context, linkedBuilder)
        {
            DocumentId = Context.VisitId;
        }

        protected override void CreateEnvelope()
        {
            Envelope = InitEnvelope();
            Envelope.DocumentTypeId = (int) DocumentType.OutletVisitNote;
        }

        protected override DocumentCommand CreateFirstCommand()
        {
            var command = InitCommand(new CreateOutletVisitNoteCommand());
            command.CommandGeneratedByCostCentreApplicationId = Context.GeneratedByCostCentreApplicationId;
            command.DocumentIssuerCostCentreId = Context.GeneratedByCostCentreId;
            command.DocumentRecipientCostCentreId = Context.RecipientCostCentreId;
            command.DocumentOnBehalfCostCentreId = Context.IssuedOnBehalfOfCostCentre;

            return command;
        }

        public override bool HasOutput { get { return true; } }

        protected override void AddConfirmCommand()
        {
            //Not used for OutletVisitNote
        }

        protected override void ProcessLineItem(BaseProductLineItem item, decimal quantity)
        {
            //Not used for OutletVisitNote
        }

        protected override void ProcessPayment(Payment payment)
        {
            //Not used for OutletVisitNote
        }
    }
}
