using System;
using System.Collections.Generic;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Repository.InventoryRepository
{
   public interface IInventoryRepository
    {
       Inventory GetById(Guid id);
       List<Inventory> GetByWareHouseId(Guid id);
       Guid AddInventory(Inventory inventory);
       Guid UpdateFromServer(Inventory inventory);
       List<Inventory> GetByProductId(Guid id);
      Inventory GetByProductIdAndWarehouseId(Guid id, Guid warehouseId);
       
       List<Inventory> GetAll();
       ValidationResultInfo Validate(Inventory itemToValidate);
       void AdjustInventoryBalance(Guid inventoryId, decimal qty,int type);
       List<Inventory> Query(Guid? warehouseId );
    }

    public interface ISourcingInventoryRepository
    {
        List<SourcingInventory> GetAll();
        List<SourcingInventory> GetByWareHouseId(Guid id);
        SourcingInventory GetByCommodityIdWarehouseIdAndGradeId(Guid commodityid,Guid gradeId, Guid warehouseId);
        void AdjustInventoryBalance(Guid costCentreId,Guid commodityid,Guid gradeId, decimal qty);
    }
}
