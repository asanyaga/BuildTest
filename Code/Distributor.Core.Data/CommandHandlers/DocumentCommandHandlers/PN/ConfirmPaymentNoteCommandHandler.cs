using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Losses;
using Distributr.Core.Data.EF;
using Distributr.Core.Repository.Transactional.DocumentRepositories.PaymentNoteRepositories;
using Distributr.Core.Workflow.FinancialWorkflow;
using log4net;
using Distributr.Core.Commands.DocumentCommands.Losses;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.PN
{
    public class ConfirmPaymentNoteCommandHandler :BaseCommandHandler, IConfirmPaymentNoteCommandHandler
    {

        IPaymentNoteRepository _documentRepository;
        ILog _log = LogManager.GetLogger("ConfirmLossCommandHandler");
        private IPaymentTrackerWorkflow _paymentTrackerWorkflow;


        public ConfirmPaymentNoteCommandHandler(CokeDataContext context, IPaymentNoteRepository documentRepository, IPaymentTrackerWorkflow paymentTrackerWorkflow) : base(context)
        {
            _documentRepository = documentRepository;
            _paymentTrackerWorkflow = paymentTrackerWorkflow;
        }

        public void Execute(ConfirmPaymentNoteCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                {
                    _log.InfoFormat("Failed to  Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                    return;
                }

                ConfirmDocument(command.DocumentId);
               
                PaymentNote pn = _documentRepository.GetById(command.DocumentId) as PaymentNote;
               
                foreach (var lineItem in pn.LineItems)
                {
                    _paymentTrackerWorkflow.AdjustAccountBalance(pn.DocumentIssuerCostCentre.Id, lineItem.PaymentMode, lineItem.Amount,pn.PaymentNoteType);
                }

            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                _log.Error("ConfirmPaymentNoteCommandHandler exception",  ex);
                throw ;
            }
        }

    }
}