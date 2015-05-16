using System;
using System.Linq;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityStorageCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityStorageCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using Distributr.Core.Workflow.InventoryWorkflow;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommoditityStorageCommandHandlers
{
    public class ConfirmCommodityStorageCommandHandler :BaseSourcingCommandHandler, IConfirmCommodityStorageCommandHandler
    {
       
        ILog _log = LogManager.GetLogger("ConfirmCommodityStorageCommandHandler");
        private ISourcingInventoryWorkflow _inventoryWorkflow;


        public ConfirmCommodityStorageCommandHandler(ISourcingInventoryWorkflow inventoryWorkflow,CokeDataContext context) : base(context)
        {
            _inventoryWorkflow = inventoryWorkflow;
        }

        public void Execute(ConfirmCommodityStorageCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                    return;
                ConfirmDocument(command.DocumentId);
                foreach (var item in _context.tblSourcingLineItem.Where(s => s.DocumentId == command.DocumentId).ToList())
                {
                    item.LineItemStatusId = (int)SourcingLineItemStatus.Confirmed;
                    _inventoryWorkflow.InventoryAdjust(item.tblSourcingDocument.DocumentRecipientCostCentreId,item.CommodityId.Value,item.GradeId.Value,item.Weight.Value);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("ConfirmCommodityStorageCommandHandler exception", ex);
                throw;
            }
        }
    }
}
