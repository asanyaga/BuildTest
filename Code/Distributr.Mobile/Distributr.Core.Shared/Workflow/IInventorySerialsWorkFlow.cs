using System.Collections.Generic;
using Distributr.Core.ClientApp;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.Core.Workflow
{
    public interface IInventorySerialsWorkFlow
    {
        void SubmitInventorySerials(List<InventorySerials> inventorySerials);
        void Save(List<InventorySerials> inventorySerials, Document document,BasicConfig config);
    }
}
