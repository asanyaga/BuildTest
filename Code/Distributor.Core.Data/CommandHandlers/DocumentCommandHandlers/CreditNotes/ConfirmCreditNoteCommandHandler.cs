using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.CreditNotes;
using Distributr.Core.Commands.DocumentCommands.CreditNotes;
using Distributr.Core.Data.EF;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using Distributr.Core.Workflow.FinancialWorkflow;
using log4net;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.FinancialEntities;
using System.Linq;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.CreditNotes
{
    public class ConfirmCreditNoteCommandHandler : BaseCommandHandler,IConfirmCreditNoteCommandHandler
    {
        private IFinancialsWorkflow _financialsWorkflow;
        private CokeDataContext _cokeDataContext;
        ILog _log = LogManager.GetLogger("ConfirmCreditNoteCommandHandler");


        public ConfirmCreditNoteCommandHandler(CokeDataContext context, IFinancialsWorkflow financialsWorkflow) : base(context)
        {
            _financialsWorkflow = financialsWorkflow;
            _cokeDataContext = context;
        }

        public void Execute(ConfirmCreditNoteCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                    return;

                ConfirmDocument(command.DocumentId);

                foreach (var li in _cokeDataContext.tblLineItems.Where(q => q.tblDocument.Id == command.DocumentId))
                {
                    //credit recipient cc account
                    _financialsWorkflow.AccountAdjust(li.tblDocument.DocumentRecipientCostCentre, AccountType.CostCentre,
                                                  li.Value.Value, DocumentType.CreditNote, li.tblDocument.Id,
                                                   li.tblDocument.DocumentDateIssued);
                    //debit issuer cash account
                    _financialsWorkflow.AccountAdjust(li.tblDocument.DocumentIssuerCostCentreId, AccountType.Cash, -li.Value.Value,
                                                 DocumentType.CreditNote, li.tblDocument.Id, li.tblDocument.DocumentDateIssued);

                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("ConfirmCreditNoteCommandHandler exception", ex);
                throw;
            }
        }
    }
}
