using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Repository.InventoryRepository
{
    public interface IInventorySerialsRepository
    {
        Guid AddInventorySerial(InventorySerials serials);
        InventorySerials GetById(Guid id);
        List<InventorySerials> GetAll();
        List<InventorySerials> GetByProductId(Guid productId);
        List<InventorySerials> GetByDocumentId(Guid documentId);
        List<InventorySerials> GetByCostCentreId(Guid costCentreId);
        ValidationResultInfo Validate(InventorySerials itemToValidate);
    }
}
