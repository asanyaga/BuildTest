using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Losses;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.PaymentNoteRepositories;
using log4net;
using Distributr.Core.Commands.DocumentCommands.Losses;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.PN
{
    public class AddPaymentNoteLineItemCommandHandler :BaseCommandHandler, IAddPaymentNoteLineItemCommandHandler
    {
     
        IPaymentNoteRepository _documentRepository;
      
        ILog _log = LogManager.GetLogger("AddLossLineItemCommandHandler");
        private CokeDataContext _context;

        public AddPaymentNoteLineItemCommandHandler(IPaymentNoteRepository documentRepository, CokeDataContext context) : base(context)
        {
            _documentRepository = documentRepository;
            _context = context;
        }

        public void Execute(AddPaymentNoteLineItemCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());

            try
            {
                if (!DocumentExists(command.DocumentId))
                {
                    _log.InfoFormat("Cannot add line item. Document does not exist. Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                    return;
                }

                if (DocumentLineItemExists(command.CommandId))
                {
                    _log.InfoFormat("Cannot add line item. Line item already exists. Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                    return;
                }
                tblDocument document = ExistingDocument(command.DocumentId);
                tblLineItems lineItem = NewLineItem(command.CommandId, command.DocumentId, null,
                                                    command.Description, 0, command.LineItemSequenceNo);


                lineItem.Value = command.Amount;

                lineItem.OrderLineItemType = (int)command.PaymentModeId;
                _context.tblLineItems.AddObject(lineItem);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("AddPaymentNoteLineItemCommandHandler exception", ex);
                throw ;
            }
        }

    }
}
