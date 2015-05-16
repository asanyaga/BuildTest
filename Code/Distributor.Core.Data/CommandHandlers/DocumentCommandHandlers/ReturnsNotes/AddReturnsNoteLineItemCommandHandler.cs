using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.ReturnsNotes;
using Distributr.Core.Commands.DocumentCommands.ReturnsNotes;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReturnsRepositories;
using log4net;
using Distributr.Core.Repository.Master.ProductRepositories;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.ReturnsNotes
{
    public class AddReturnsNoteLineItemCommandHandler :BaseCommandHandler, IAddReturnsNoteLineItemCommandHandler
    {
        private CokeDataContext _cokeDataContext;
        ILog _log = LogManager.GetLogger("AddReturnsNoteLineItemCommandHandler");


        public AddReturnsNoteLineItemCommandHandler(CokeDataContext context, CokeDataContext cokeDataContext) : base(context)
        {
            _cokeDataContext = cokeDataContext;
        }

        public void Execute(AddReturnsNoteLineItemCommand command)
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
                tblLineItems lineItem = NewLineItem(command.CommandId, command.DocumentId, command.ProductId,
                                                    command.Description, command.Expected, command.LineItemSequenceNo);
                lineItem.IAN_Actual = command.Actual;
                lineItem.Receipt_PaymentTypeId = command.ReturnTypeId;
                lineItem.OrderLineItemType = command.LossType;
                lineItem.ReturnsNoteReason = command.Reason;
                lineItem.Value = command.Value;
                lineItem.Other = command.Other;
                document.tblLineItems.Add(lineItem);
                _cokeDataContext.SaveChanges();
              
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("AddReturnsNoteLineItemCommandHandler exception", ex);
                throw;
            }
        }
    }
}
