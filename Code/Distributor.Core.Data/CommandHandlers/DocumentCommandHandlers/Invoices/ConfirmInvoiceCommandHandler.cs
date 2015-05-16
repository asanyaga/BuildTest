using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Invoices;
using Distributr.Core.Commands.DocumentCommands.Invoices;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using Distributr.Core.Workflow.FinancialWorkflow;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Invoices
{
    public class ConfirmInvoiceCommandHandler : BaseCommandHandler, IConfirmInvoiceCommandHandler
    {
        private IFinancialsWorkflow _financialsWorkflow;
        private IInvoiceRepository _documentRepository;
        private CokeDataContext _cokeDataContext;
        ILog _log = LogManager.GetLogger("ConfirmInvoiceCommandHandler");

        public ConfirmInvoiceCommandHandler(IFinancialsWorkflow financialsWorkflow, IInvoiceRepository documentRepository, CokeDataContext cokeDataContext) 
            : base(cokeDataContext)
        {
            _financialsWorkflow = financialsWorkflow;
            _documentRepository = documentRepository;
            _cokeDataContext = cokeDataContext;
        }

        public void Execute(ConfirmInvoiceCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                    return;

                ConfirmDocument(command.DocumentId);

                Invoice invoice = _documentRepository.GetById(command.DocumentId) as Invoice;
                
                //debit recipient cc account

                _financialsWorkflow.AccountAdjust(invoice.DocumentRecipientCostCentre.Id, AccountType.CostCentre,
                                                  -invoice.TotalGross, DocumentType.Invoice, invoice.Id,
                                                  invoice.DocumentDateIssued);
                //credit issuer cc sale account
                _financialsWorkflow.AccountAdjust(invoice.DocumentIssuerCostCentre.Id, AccountType.Sales, invoice.TotalNet,
                                                  DocumentType.Invoice, invoice.Id, invoice.DocumentDateIssued);
                //credit issuer tax account
                _financialsWorkflow.AccountAdjust(invoice.DocumentIssuerCostCentre.Id, AccountType.SalesTax, invoice.TotalVat,
                                                  DocumentType.Invoice, invoice.Id, invoice.DocumentDateIssued);

            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("ConfirmInvoiceCommandHandler exception",ex);
                throw ;
            }
        }
    }
}
