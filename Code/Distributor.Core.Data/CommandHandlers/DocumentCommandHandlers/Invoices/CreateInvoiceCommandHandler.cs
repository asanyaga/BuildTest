using Distributr.Core.CommandHandler.DocumentCommandHandlers.Invoices;
using Distributr.Core.Commands.DocumentCommands.Invoices;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using log4net;
using System;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Invoices
{
    public class CreateInvoiceCommandHandler :BaseCommandHandler, ICreateInvoiceCommandHandler
    {
        private CokeDataContext _cokeDataContext;
        ILog _log = LogManager.GetLogger("CreateInvoiceCommandHandler");

        public CreateInvoiceCommandHandler(CokeDataContext cokeDataContext) : base(cokeDataContext)
        {
            _cokeDataContext = cokeDataContext;
        }

        public void Execute(CreateInvoiceCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (DocumentExists(command.DocumentId))
                    return;

                tblDocument doc = NewDocument(command, DocumentType.Invoice, command.DocumentRecipientCostCentreId);
                doc.InvoiceOrderId = command.OrderId;
                doc.SaleDiscount = command.SaleDiscount;
                _cokeDataContext.tblDocument.AddObject(doc);
                _cokeDataContext.SaveChanges();
               
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("CreateInvoiceCommandHandler exception " ,ex);
                throw ;
            }
        }
    }
}
