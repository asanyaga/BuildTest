using System;
using System.Linq;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityWarehouseStorageCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityWarehouseStorageCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityWarehouseStorageCommandHandlers
{
    public class GenerateReceiptCommodityWarehouseStorageCommandHandler : BaseSourcingCommandHandler, IGenerateReceiptCommodityWarehouseStorageCommandHandler
    {
        ILog _log = LogManager.GetLogger("GenerateReceiptCommodityWarehouseStorageCommandHandler");
         public GenerateReceiptCommodityWarehouseStorageCommandHandler(CokeDataContext context)
             : base(context)
        {
        }

         public void Execute(GenerateReceiptCommodityWarehouseStorageCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                    return;
                tblSourcingDocument document = ExistingDocument(command.DocumentId);
                document.DocumentStatusId = (int)DocumentSourcingStatus.ReceiptGenerated;
                document.IM_DateLastUpdated = DateTime.Now;
               // document.DocumentOnBehalfOfCostCentreId = command.StoreId;
                foreach(var item in _context.tblSourcingLineItem.Where(s=>s.DocumentId==command.DocumentId))
                {
                    item.LineItemStatusId =(int) SourcingLineItemStatus.ReceiptGenerated;
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("GenerateReceiptCommodityWarehouseStorageCommandHandler exception", ex);
                throw;
            }
        }
    }
}
