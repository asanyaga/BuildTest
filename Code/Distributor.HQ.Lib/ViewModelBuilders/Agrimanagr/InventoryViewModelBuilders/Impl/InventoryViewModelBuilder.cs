using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.HQ.Lib.ViewModels.Agrimanagr.InventoryViewModel;

namespace Distributr.HQ.Lib.ViewModelBuilders.Agrimanagr.InventoryViewModelBuilders.Impl
{
    public class InventoryViewModelBuilder : IInventoryViewModelBuilder
    {
        private ISourcingInventoryRepository _sourcingInventoryRepository;

        public InventoryViewModelBuilder(ISourcingInventoryRepository sourcingInventoryRepository)
        {
            _sourcingInventoryRepository = sourcingInventoryRepository;
        }

        public List<InventoryLevelHQViewModel> GetAll()
        {
            return _sourcingInventoryRepository.GetAll().Select(Map).ToList();
        }

        private InventoryLevelHQViewModel Map(SourcingInventory sourcingInventory)
        {
            return new InventoryLevelHQViewModel()
            {
                Commodity = sourcingInventory.Commodity,
                Grade = sourcingInventory.Grade,
                Warehouse = sourcingInventory.Warehouse,
                Balance = sourcingInventory.Balance,
                Value = sourcingInventory.Value
            };
        }
    }
}
