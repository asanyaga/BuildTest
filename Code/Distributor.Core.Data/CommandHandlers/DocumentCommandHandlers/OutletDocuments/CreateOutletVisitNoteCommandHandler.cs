using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.OutletDocument;
using Distributr.Core.Commands.DocumentCommands.OutletDocument;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.OutletDocuments
{
    public class CreateOutletVisitNoteCommandHandler :BaseCommandHandler, ICreateOutletVisitNoteCommandHandler
    {
        ILog _log = LogManager.GetLogger("CreateOutletVisitNoteCommandHandler");

        private CokeDataContext _cokeDataContext;


        public CreateOutletVisitNoteCommandHandler(CokeDataContext context) : base(context)
        {
            _cokeDataContext = context;
        }

        public void Execute(CreateOutletVisitNoteCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (DocumentExists(command.DocumentId))
                    return;

                tblDocument doc = NewDocument(command, DocumentType.OutletVisitNote, command.DocumentRecipientCostCentreId);
                doc.Note = command.Description;
                doc.OrderIssuedOnBehalfOfCC = command.DocumentOnBehalfCostCentreId;
                doc.PaymentDocId = command.ReasonId;
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
