using System;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityWarehouseStorageCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityWarehouseStorageCommands;
using Distributr.Core.Data.EF;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityWarehouseStorageCommandHandlers
{
    public class AddCommodityWarehouseStorageLineItemCommandHandler :BaseSourcingCommandHandler, IAddCommodityWarehouseStorageLineItemCommandHandler
    {
        ILog _log = LogManager.GetLogger("AddCommodityWarehouseStorageLineItemCommandHandler");
        public AddCommodityWarehouseStorageLineItemCommandHandler(CokeDataContext context)
            : base(context)
        {
        }

        public void Execute(AddCommodityWarehouseStorageLineItemCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                {
                    _log.InfoFormat("Cannot add line item. Document does not exist  Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                    return;
                }

                if (DocumentLineItemExists(command.DocumentLineItemId))
                {
                    _log.InfoFormat("Cannot add line item {0}. Line item already exists", command.DocumentLineItemId);
                    return;
                }
                tblSourcingDocument doc = ExistingDocument(command.DocumentId);
                tblSourcingLineItem lineItem = NewLineItem(command.DocumentLineItemId, command.ParentLineItemId,command.DocumentId, command.CommodityId,command.CommodityGradeId,command.ContainerTypeId,command.Weight,command.Description,command.ContainerNo);
               doc.tblSourcingLineItem.Add(lineItem);
                lineItem.NoOfContainer = command.NoOfContainers;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("AddCommodityWarehouseStorageLineItemCommandHandler exception", ex);
                throw;
            }
        }
    }
}
