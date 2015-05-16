using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.DispatchNotes;
using Distributr.Core.Commands.DocumentCommands.DispatchNotes;
using Distributr.Core.Data.EF;
using Distributr.Core.Factory.Documents;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Transactional.DocumentRepositories.DispatchRepositories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.DispatchNotes
{
    public class AddDispatchNoteLineItemCommandHandler : BaseCommandHandler, IAddDispatchNoteLineItemCommandHandler
    {
        ILog _log = LogManager.GetLogger("AddDispatchNoteLineItemCommandHandler");
        private CokeDataContext _cokeDataContext;


        public AddDispatchNoteLineItemCommandHandler(CokeDataContext cokeDataContext) : base(cokeDataContext)
        {
            _cokeDataContext = cokeDataContext;
        }

        public void Execute(AddDispatchNoteLineItemCommand command)
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
                 lineItem.OrderLineItemType = command.LineItemType;
                 lineItem.ProductDiscount = command.LineItemProductDiscount;
                 lineItem.DiscountLineItemTypeId = command.DiscountType;

                 doc.tblLineItems.Add(lineItem);
                 _cokeDataContext.SaveChanges();
             }
             catch (Exception ex)
             {
                 _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                 //_log.Error(ex);
                 throw ;
             }
        }

    }
}
