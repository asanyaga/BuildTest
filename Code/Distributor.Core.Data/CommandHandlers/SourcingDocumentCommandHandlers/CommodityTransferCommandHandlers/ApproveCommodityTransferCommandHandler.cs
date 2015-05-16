using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityTranferCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityTransferCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Workflow.InventoryWorkflow;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityTransferCommandHandlers
{
    public class ApproveCommodityTransferCommandHandler : BaseSourcingCommandHandler, IApproveCommodityTransferCommandHandler
    {
        ILog _log = LogManager.GetLogger("ApproveCommodityTransferCommandHandler");
        private ISourcingInventoryWorkflow _inventoryWorkflow;

        public ApproveCommodityTransferCommandHandler(CokeDataContext context, ISourcingInventoryWorkflow inventoryWorkflow) : base(context)
        {
            _inventoryWorkflow = inventoryWorkflow;
        }

        public void Execute(ApproveCommodityTransferCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                    return;
                ApproveDocument(command.DocumentId);
                foreach (var item in _context.tblSourcingLineItem.Where(s => s.DocumentId == command.DocumentId).ToList())
                {
                    //var store = (Guid) item.tblSourcingDocument.DocumentOnBehalfOfCostCentreId;

                    _inventoryWorkflow.InventoryAdjust(command.WareHouseId, item.CommodityId.Value, item.GradeId.Value, item.Weight.Value);
                    /*_inventoryWorkflow.InventoryAdjust(item.tblSourcingDocument.DocumentRecipientCostCentreId, 
                                                       item.CommodityId.Value, item.GradeId.Value, -item.Weight.Value);*/
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("ApproveCommodityTransferCommandHandler exception", ex);
                throw;
            }
        }
    }
}
