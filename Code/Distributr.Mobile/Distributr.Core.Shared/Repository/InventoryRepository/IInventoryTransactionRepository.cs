using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Repository.InventoryRepository
{
    public interface IInventoryTransactionRepository
    {
        InventoryTransaction GetById(Guid id);
        Guid Add(InventoryTransaction inventoryTransaction);
        List<InventoryTransaction> GetByWarehouse(Guid wareHouseId);
        List<InventoryTransaction> GetByWarehouse(Guid wareHouseId, Inventory inventory, DocumentType? documentType);
        List<InventoryTransaction> GetByWarehouse(Guid wareHouseId, Inventory inventory, DocumentType? documentType, DateTime startDate, DateTime endDate);
        List<InventoryTransaction> GetByDate(DateTime startDate, DateTime endDate);
        ValidationResultInfo Validate(InventoryTransaction itemToValidate);
    }
}
