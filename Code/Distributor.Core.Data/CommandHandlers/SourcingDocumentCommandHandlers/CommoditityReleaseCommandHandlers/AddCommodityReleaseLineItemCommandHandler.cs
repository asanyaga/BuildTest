using System;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityPurchaseCommandHandler;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityReleaseCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityPurchaseCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityReleaseCommands;
using Distributr.Core.Data.EF;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommoditityReleaseCommandHandlers
{
    public class AddCommodityReleaseLineItemCommandHandler :BaseSourcingCommandHandler, IAddCommodityReleaseLineItemCommandHandler
    {

        ILog _log = LogManager.GetLogger("AddCommodityReleaseLineItemCommandHandler");


        public AddCommodityReleaseLineItemCommandHandler(CokeDataContext context)
            : base(context)
        {
        }

        public void Execute(AddCommodityReleaseNoteLineItemCommand command)
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
                tblSourcingDocument doc = ExistingDocument(command.DocumentId);
                tblSourcingLineItem lineItem = NewLineItem(command.DocumentLineItemId, command.ParentLineItemId, command.DocumentId, command.CommodityId, command.CommodityGradeId, command.ContainerTypeId, command.Weight, command.Description, command.ContainerNo);
                doc.tblSourcingLineItem.Add(lineItem);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("AddCommodityReleaseLineItemCommandHandler exception", ex);
                throw;
            }
        }
    }
}
