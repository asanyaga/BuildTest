﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityTranferCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityTransferCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;
using Distributr.Core.Workflow.InventoryWorkflow;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityTransferCommandHandlers
{
    public class ConfirmCommodityTransferCommandHandler : BaseSourcingCommandHandler, IConfirmCommodityTransferCommandHandler
    {
       
        ILog _log = LogManager.GetLogger("ConfirmCommodityTransferCommandHandler");
        private ISourcingInventoryWorkflow _inventoryWorkflow;

        public ConfirmCommodityTransferCommandHandler(CokeDataContext context, ISourcingInventoryWorkflow inventoryWorkflow)
            : base(context)
        {
            _inventoryWorkflow = inventoryWorkflow;
        }

        public void Execute(ConfirmCommodityTransferCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                    return;
                ConfirmDocument(command.DocumentId);
                var lineItems = _context.tblSourcingLineItem.Where(s => s.DocumentId == command.DocumentId).ToList();
                foreach (var item in lineItems)
                {
                    /*_inventoryWorkflow.InventoryAdjust(item.tblSourcingDocument.DocumentRecipientCostCentreId, 
                        item.CommodityId.Value, item.GradeId.Value, item.Weight.Value);*/
                    _inventoryWorkflow.InventoryAdjust(item.tblSourcingDocument.DocumentIssuerCostCentreId,
                        item.CommodityId.Value, item.GradeId.Value, decimal.Negate(item.Weight.Value));
                   
                }
                foreach (var sourcingItemId in lineItems.Select(s=>s.ParentId).ToList())
                {
                    var item = _context.tblSourcingLineItem.FirstOrDefault(s => s.Id == sourcingItemId);
                    item.LineItemStatusId = (int)SourcingLineItemStatus.Transfered;
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("ConfirmCommodityTransferCommandHandler exception", ex);
                throw;
            }
        }
    }
}