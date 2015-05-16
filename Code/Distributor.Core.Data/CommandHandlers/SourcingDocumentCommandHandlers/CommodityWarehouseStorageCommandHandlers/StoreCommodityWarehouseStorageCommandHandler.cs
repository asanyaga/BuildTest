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
    public class StoreCommodityWarehouseStorageCommandHandler : BaseSourcingCommandHandler, IStoreCommodityWarehouseStorageCommandHandler
    {
         ILog _log = LogManager.GetLogger("StoreCommodityWarehouseStorageCommandHandler");
         public StoreCommodityWarehouseStorageCommandHandler(CokeDataContext context)
             : base(context)
        {
        }

        public void Execute(StoreCommodityWarehouseStorageCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                    return;
                tblSourcingDocument document = ExistingDocument(command.DocumentId);
                document.DocumentStatusId = (int)DocumentSourcingStatus.Closed; document.IM_DateLastUpdated = DateTime.Now;
                document.DocumentOnBehalfOfCostCentreId = command.StoreId;
                AdjustInventory(command.DocumentId,command.StoreId);
                
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("StoreCommodityWarehouseStorageCommandHandler exception", ex);
                throw;
            }
        }

         private void AdjustInventory(Guid documentId,Guid storeId)
        {
            tblSourcingDocument document = ExistingDocument(documentId);
            foreach (var lineItem in document.tblSourcingLineItem)
            {

                var inventory = _context.tblCommoditySupplierInventory.FirstOrDefault(p=>p.CommodityId==lineItem.CommodityId && p.GradeId==lineItem.GradeId && p.WareHouseId==storeId && p.SupplierId==document.DocumentRecipientCostCentreId);
                if(inventory==null)
                {
                    inventory=new tblCommoditySupplierInventory();
                    inventory.id = Guid.NewGuid();
                    inventory.CommodityId = lineItem.CommodityId.Value;
                    inventory.GradeId = lineItem.GradeId.Value;
                    inventory.WareHouseId = storeId;
                    inventory.SupplierId = document.DocumentRecipientCostCentreId;
                    inventory.IM_Status = 0;
                    inventory.Balance = 0;
                    inventory.IM_DateCreated = DateTime.Now;
                    _context.tblCommoditySupplierInventory.AddObject(inventory);

                }
                var balance = lineItem.Weight.Value - lineItem.FinalWeight.Value;
                inventory.Balance += balance;
                inventory.IM_DateLastUpdated = DateTime.Now;
                _context.SaveChanges();
            }
        }
    }
}
