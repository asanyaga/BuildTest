using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Losses;
using Distributr.Core.Data.EF;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.PaymentNoteRepositories;
using log4net;
using Distributr.Core.Commands.DocumentCommands.Losses;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.PN
{
    public class CreatePaymentNoteCommandHandler :BaseCommandHandler, ICreatePaymentNoteCommandHandler
    {

       
        ILog _log = LogManager.GetLogger("CreateLossCommandHandler");
        private CokeDataContext _context;

        public CreatePaymentNoteCommandHandler(CokeDataContext context) : base(context)
        {
            _context = context;
        }

        public void Execute(CreatePaymentNoteCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());

            try
            {
                if (DocumentExists(command.DocumentId))
                    return;

                tblDocument doc = NewDocument(command, DocumentType.PaymentNote,
                                              command.PaymentNoteRecipientCostCentreId);
                doc.DocumentDateIssued = command.DocumentDateIssued;
                doc.DocumentIssuerCostCentreApplicationId = command.CommandGeneratedByCostCentreApplicationId;
                doc.SendDateTime = command.SendDateTime;
                doc.OrderOrderTypeId = (int)command.PaymentNoteTypeId;
                doc.SendDateTime = command.SendDateTime;
                doc.DocumentDateIssued = command.CommandCreatedDateTime;
                _context.tblDocument.AddObject(doc);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("CreatePaymentNoteCommandHandler exception", ex);
                throw ;
            }
        }

    }
}
