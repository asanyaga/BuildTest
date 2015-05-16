using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.ReturnsNotes;
using Distributr.Core.Commands.DocumentCommands.ReturnsNotes;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReturnsRepositories;
using Distributr.Core.Workflow.FinancialWorkflow;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.ReturnsNotes
{
    public class CloseReturnsNoteCommandHandler :BaseCommandHandler, ICloseReturnsNoteCommandHandler
    {
        private IReturnsNoteRepository _documentRepository;
        private IFinancialsWorkflow _financialsWorkflow;
        ILog _log = LogManager.GetLogger("CloseReturnsNoteCommandHandler");

        public CloseReturnsNoteCommandHandler(CokeDataContext context, IReturnsNoteRepository documentRepository, IFinancialsWorkflow financialsWorkflow) : base(context)
        {
            _documentRepository = documentRepository;
            _financialsWorkflow = financialsWorkflow;
        }

        public void Execute(CloseReturnsNoteCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                {
                    _log.InfoFormat("Failed to  Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                    return;
                }

                CloseDocument(command.DocumentId);
                ReturnsNote rn = _documentRepository.GetById(command.DocumentId) as ReturnsNote;

                foreach (var li in rn._lineItems)
                {
                    if (li.ReturnType != ReturnsType.Inventory)
                    {
                        _log.Info(
                            string.Format("Adjusting finacial Returns Note ID {0} for Recipient costcentre  {1}", rn.Id,
                                rn.DocumentRecipientCostCentre.Name));
                        //credit recipient cc account
                        _financialsWorkflow.AccountAdjust(rn.DocumentRecipientCostCentre.Id, AccountType.CostCentre,
                            li.Value, DocumentType.ReturnsNote, rn.Id,
                            rn.DocumentDateIssued);
                       
                        //debit issuer cash account
                        _financialsWorkflow.AccountAdjust(rn.DocumentIssuerCostCentre.Id, AccountType.Cash, -li.Value,
                            DocumentType.ReturnsNote, rn.Id, rn.DocumentDateIssued);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("CloseReturnsNoteCommandHandler exception", ex);
                throw ;
            }
        }
    }
}
