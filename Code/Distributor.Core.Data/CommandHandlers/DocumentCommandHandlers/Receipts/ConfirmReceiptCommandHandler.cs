using System;
using System.Linq;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Receipts;
using Distributr.Core.Commands.DocumentCommands.Receipts;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using Distributr.Core.Workflow.FinancialWorkflow;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Receipts
{
    public class ConfirmReceiptCommandHandler :BaseCommandHandler,IConfirmReceiptCommandHandler
    {
        private IFinancialsWorkflow _financialsWorkflow;
        private IReceiptRepository _receiptRepository; 
        private CokeDataContext _cokeDataContext;
        ILog _log = LogManager.GetLogger("ConfirmReceiptCommandHandler");

        public ConfirmReceiptCommandHandler(IFinancialsWorkflow financialsWorkflow, CokeDataContext context, IReceiptRepository receiptRepository)
            :base(context)
        {
            _cokeDataContext = context;
            _financialsWorkflow = financialsWorkflow;
            _receiptRepository =receiptRepository;
        }

        public void Execute(ConfirmReceiptCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                    return;

                ConfirmDocument(command.DocumentId);
               
               Receipt r= _receiptRepository.GetById(command.DocumentId);
                    
                foreach (var li in r.LineItems)
                {
                    //credit recipient cc account
                    _financialsWorkflow.AccountAdjust(r.DocumentRecipientCostCentre.Id, AccountType.CostCentre,
                                                  li.Value, DocumentType.Receipt, r.Id,
                                                  r.DocumentDateIssued);
                    //debit issuer cash account
                    _financialsWorkflow.AccountAdjust(r.DocumentIssuerCostCentre.Id, AccountType.Cash, -li.Value,
                                                 DocumentType.Receipt, r.Id, r.DocumentDateIssued);

                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("ConfirmReceiptCommandHandler exception",  ex);
                throw ;
            }

        }


    }
}
