using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.InventoryEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Repository.Master.CommodityRepositories;
using Distributr.Core.Repository.Master.CostCentreRepositories;

namespace Distributr.Core.Data.Repository.InventoryRepository
{
    public class SourcingInventoryRepository : ISourcingInventoryRepository
    {
        CokeDataContext _ctx;
        private ICommodityRepository _commodityRepository;
        private ICostCentreRepository _costCentreRepository;

        public SourcingInventoryRepository(CokeDataContext ctx, ICommodityRepository commodityRepository, ICostCentreRepository costCentreRepository)
        {
            _ctx = ctx;
            _commodityRepository = commodityRepository;
            _costCentreRepository = costCentreRepository;
        }


        public List<SourcingInventory> GetAll()
        {
            return _ctx.tblSourcingInventory.ToList().Select(Map).ToList();
        }

        public List<SourcingInventory> GetByWareHouseId(Guid id)
        {
            var data= _ctx.tblSourcingInventory.Where(s => s.WareHouseId == id).ToList().Select(Map).ToList();
            return data.OrderBy(s => s.Warehouse.Name).ThenBy(s => s.Commodity.Name).ThenBy(s => s.Grade.Name).ToList();

        }

        public SourcingInventory GetByCommodityIdWarehouseIdAndGradeId(Guid commodityid, Guid gradeId, Guid warehouseId)
        {
            var inventory =
              _ctx.tblSourcingInventory.FirstOrDefault(s => s.CommodityId == commodityid && s.GradeId == gradeId && s.WareHouseId == warehouseId);
          if(inventory!=null)
          {
              return Map(inventory);
          }
            return null;
        }

        private SourcingInventory Map(tblSourcingInventory inventory)
        {
            return new SourcingInventory(inventory.id)
                       {
                           Balance = inventory.Balance.Value,
                           UnavailableBalance = inventory.UnavailableBalance,
                           Value = 0,
                           Commodity = _commodityRepository.GetById(inventory.CommodityId),
                           Warehouse = _costCentreRepository.GetById(inventory.WareHouseId) as Warehouse,
                           Grade = _commodityRepository.GetGradeByGradeId(inventory.GradeId),
                          
                       };
        }

        public void AdjustInventoryBalance(Guid costCentreId,Guid commodityid, Guid gradeId, decimal qty)
        {
            DateTime date = DateTime.Now;
            var inventory =
                _ctx.tblSourcingInventory.FirstOrDefault(s => s.CommodityId == commodityid && s.GradeId == gradeId && s.WareHouseId== costCentreId);
            if (inventory == null)
            {
                inventory = new tblSourcingInventory();
                inventory.id = Guid.NewGuid();
                inventory.GradeId = gradeId;
                inventory.CommodityId = commodityid;
                inventory.WareHouseId = costCentreId;
                inventory.IM_DateCreated = date;
                inventory.IM_Status = 1;
                inventory.Balance = 0;
                inventory.UnavailableBalance = 0;
                _ctx.tblSourcingInventory.AddObject(inventory);
               
            }
            inventory.Balance += qty;
            inventory.IM_DateLastUpdated = date;
            _ctx.SaveChanges();
        }
    }
}
