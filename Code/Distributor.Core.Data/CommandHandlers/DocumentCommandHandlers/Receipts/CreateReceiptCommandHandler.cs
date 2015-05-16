using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Receipts;
using Distributr.Core.Commands.DocumentCommands.Receipts;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Receipts
{
    public class CreateReceiptCommandHandler :BaseCommandHandler, ICreateReceiptCommandHandler
    {
       
        ILog _log = LogManager.GetLogger("CreateReceiptCommandHandler");

        private CokeDataContext _cokeDataContext;


        public CreateReceiptCommandHandler(CokeDataContext context) : base(context)
        {
            _cokeDataContext = context;
        }

        public void Execute(CreateReceiptCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (DocumentExists(command.DocumentId))
                    return;

                tblDocument doc = NewDocument(command, DocumentType.Receipt, command.DocumentRecipientCostCentreId);
                doc.InvoiceOrderId = command.InvoiceId;
                doc.PaymentDocId = command.PaymentDocId;

                _cokeDataContext.tblDocument.AddObject(doc);
                _cokeDataContext.SaveChanges();

            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("CreateReceiptCommandHandler exception ", ex);
                throw;
            }
        }

    }
}
