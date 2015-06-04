using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Commands.CommandPackage;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Mobile.Core.OrderSale;

namespace Distributr.Mobile.Core.Envelopes
{
    public abstract class BaseEnvelopeBuilder : IEnvelopeBuilder
    {
        protected readonly List<DocumentCommand> Commands = new List<DocumentCommand>();
        protected CommandEnvelope Envelope { get; set; }
        protected IEnvelopeBuilder LinkedBuilder { get; set; }
        protected IEnvelopeContext Context { get; set; }
        protected Guid DocumentId { get; set; }

        protected BaseEnvelopeBuilder(IEnvelopeContext context): this(context, new NoOpEnvelopeBuilder())
        {
        }

        protected BaseEnvelopeBuilder(IEnvelopeContext context, IEnvelopeBuilder linkedBuilder)
        {
            LinkedBuilder = linkedBuilder;
            Context = context;
            DocumentId = Guid.NewGuid();
        }

        protected CommandEnvelope InitEnvelope()
        {
            var envelope = new CommandEnvelope()
            {
                Id = Guid.NewGuid(),
                DocumentId = DocumentId,
                ParentDocumentId = Context.ParentDocumentId,
                GeneratedByCostCentreId = Context.GeneratedByCostCentreId,
                RecipientCostCentreId = Context.RecipientCostCentreId,
                GeneratedByCostCentreApplicationId = Context.GeneratedByCostCentreApplicationId,
                EnvelopeGeneratedTick = Context.Timestamp.Ticks,
            };
            
            envelope.OtherRecipientCostCentreList.Add(Context.RecipientCostCentreId);
            
            return envelope;
        }

        protected T InitCommand<T>(T command) where T : DocumentCommand
        {
            command.CommandId = Guid.NewGuid();
            command.PDCommandId = Context.ParentDocumentId;
            command.DocumentId = DocumentId;
            command.CommandCreatedDateTime = Context.Timestamp;
            command.CommandGeneratedByCostCentreApplicationId = Context.GeneratedByCostCentreApplicationId;
            command.CommandGeneratedByUserId = Context.GeneratedByUserId;
            command.CommandGeneratedByCostCentreId = Context.GeneratedByCostCentreId;
            command.CommandGeneratedByCostCentreApplicationId = Context.GeneratedByCostCentreApplicationId;
            command.SendDateTime = Context.Timestamp;
            command.CommandCreatedDateTime = Context.Timestamp;
            var createCommand = command as CreateCommand;
            if (createCommand == null) return command;

            createCommand.DocumentIssuerCostCentreId = Context.GeneratedByCostCentreId;
            createCommand.DocIssuerUserId = Context.GeneratedByUserId;
            createCommand.DocumentDateIssued = Context.Timestamp;

            return command;
        }

        public void Handle(BaseProductLineItem item, decimal quantity)
        {
            ProcessLineItem(item, quantity);
            LinkedBuilder.Handle(item, quantity);
        }

        public void Handle(Payment payment)
        {
            ProcessPayment(payment);
            LinkedBuilder.Handle(payment);
        }

        public virtual bool HasOutput
        {
            get
            {
                return Commands.Any();
            }            
        }

        protected abstract void CreateEnvelope();
        protected abstract DocumentCommand CreateFirstCommand();
        protected abstract void AddConfirmCommand();
        protected abstract void ProcessLineItem(BaseProductLineItem item, decimal quantity);
        protected abstract void ProcessPayment(Payment payment);

        public List<CommandEnvelope> Build()
        {
            if (!HasOutput)
            {
                return LinkedBuilder.Build();
            }

            CreateEnvelope();
            Commands.Insert(0, CreateFirstCommand());
            AddConfirmCommand();

            var index = 1;
            Commands.ForEach(c => Envelope.CommandsList.Add(new CommandEnvelopeItem(index++, c)));

            var envelopes = new List<CommandEnvelope>()
            {
                Envelope
            };

            envelopes.AddRange(LinkedBuilder.Build());

            return envelopes;
        }
    }
}
