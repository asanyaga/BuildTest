using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Repository.InventoryRepository;

namespace Distributr.Core.Workflow.InventoryWorkflow.Impl
{
   public class SourcingInventoryWorkflow : ISourcingInventoryWorkflow
   {
       private ISourcingInventoryRepository _inventoryRepository;

       public SourcingInventoryWorkflow(ISourcingInventoryRepository inventoryRepository)
       {
           _inventoryRepository = inventoryRepository;
       }

       public void InventoryAdjust(Guid costCentreId, Guid commodityId,Guid gradeId, decimal qty)
       {
           _inventoryRepository.AdjustInventoryBalance(costCentreId,commodityId, gradeId, qty);
           
       }
    }
}
