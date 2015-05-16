using System;
using System.Linq;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityWarehouseStorageCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityWarehouseStorageCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityWarehouseStorageCommandHandlers
{
    public class UpdateCommodityWarehouseStorageLineItemCommandHandler : BaseSourcingCommandHandler, IUpdateCommodityWarehouseStorageLineItemCommandHandler
    {
        ILog _log = LogManager.GetLogger("UpdateCommodityWarehouseStorageLineItemCommandHandler");
        public UpdateCommodityWarehouseStorageLineItemCommandHandler(CokeDataContext context)
            : base(context)
        {
        }

        public void Execute(UpdateCommodityWarehouseStorageLineItemCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                {
                    _log.InfoFormat("Cannot update line item. Document does not exist  Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                    return;
                }

                if (!DocumentLineItemExists(command.DocumentLineItemId))
                {
                    _log.InfoFormat("Cannot update line item {0}. Line item does not exist", command.CommandId);
                    return;
                }
                tblSourcingDocument doc = ExistingDocument(command.DocumentId);
                tblSourcingLineItem lineItem =
                    _context.tblSourcingLineItem.FirstOrDefault(p => p.Id == command.DocumentLineItemId);
                lineItem.FinalWeight = command.Weight;
                lineItem.LineItemStatusId=(int) SourcingLineItemStatus.Received;
                //doc.DocumentStatusId =(int) DocumentSourcingStatus.Approved;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("UpdateCommodityWarehouseStorageLineItemCommandHandler exception", ex);
                throw;
            }
        }
    }
}