using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.CreditNotes;
using Distributr.Core.Commands.DocumentCommands.CreditNotes;
using Distributr.Core.Data.EF;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.CreditNoteRepositories;
using log4net;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.CreditNotes
{
    public class AddCreditNoteLineItemCommandHandler : BaseCommandHandler,IAddCreditNoteLineItemCommandHandler
    {
        
        ILog _log = LogManager.GetLogger("AddCreditNoteLineItemCommandHandler");
        private CokeDataContext _cokeDataContext;


        public AddCreditNoteLineItemCommandHandler(CokeDataContext context) : base(context)
        {
            _cokeDataContext = context;
        }

        public void Execute(AddCreditNoteLineItemCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                {
                    _log.InfoFormat("Cannot add line item. Document does not exist  Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                    return;
                }
                if (DocumentLineItemExists(command.CommandId))
                {
                    _log.InfoFormat("Cannot add line item {0}. Line item already exists", command.CommandId);
                    return;
                }

                tblDocument doc = ExistingDocument(command.DocumentId);
                tblLineItems lineItem = NewLineItem(command.CommandId, command.DocumentId, command.ProductId,
                                                    command.Description, command.Qty, command.LineItemSequenceNo);
                lineItem.Value = command.Value;
               
                lineItem.Vat = command.LineItemVatValue;
                doc.tblLineItems.Add(lineItem);
                _cokeDataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("AddInvoiceLineItemCommandHandler exception", ex);
                throw;
            }
          
        }
    }
}
