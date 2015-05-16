using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Invoices;
using Distributr.Core.Commands.DocumentCommands.Invoices;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Invoices
{
    public class CloseInvoiceCommandHandler :BaseCommandHandler,  ICloseInvoiceCommandHandler
    {
        ILog _log = LogManager.GetLogger("CloseInvoiceCommandHandler");
        private CokeDataContext _cokeDataContext;
        public CloseInvoiceCommandHandler(IInvoiceRepository documentRepository, CokeDataContext cokeDataContext) 
            : base(cokeDataContext)
        {
            _cokeDataContext = cokeDataContext;
        }

        public void Execute(CloseInvoiceCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                    return;
                CloseDocument(command.DocumentId);
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("CloseInvoiceCommandHandler exception", ex);
                throw;
            }
        }
    }
}
