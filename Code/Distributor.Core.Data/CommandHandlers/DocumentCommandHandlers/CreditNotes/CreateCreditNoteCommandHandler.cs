using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.CreditNotes;
using Distributr.Core.Data.EF;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using log4net;
using Distributr.Core.Commands.DocumentCommands.CreditNotes;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.CreditNotes
{
    public class CreateCreditNoteCommandHandler : BaseCommandHandler,ICreateCreditNoteCommandHandler
    {
        private CokeDataContext _cokeDataContext;
        ILog _log = LogManager.GetLogger("CreateCreditNoteCommandHandler");


        public CreateCreditNoteCommandHandler(CokeDataContext context) : base(context)
        {
            _cokeDataContext = context;
        }

        public void Execute(CreateCreditNoteCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (DocumentExists(command.DocumentId))
                    return;

                tblDocument doc = NewDocument(command, DocumentType.CreditNote, command.DocumentRecipientCostCentreId);
                doc.InvoiceOrderId = command.InvoiceId;
               _cokeDataContext.tblDocument.AddObject(doc);
                _cokeDataContext.SaveChanges();

            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("CreateInvoiceCommandHandler exception ", ex);
                throw;
            }
           
        }
    }
}
