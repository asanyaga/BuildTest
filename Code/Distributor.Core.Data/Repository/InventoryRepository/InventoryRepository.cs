using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master;
using Distributr.Core.Repository.InventoryRepository;
using Distributr.Core.Domain.InventoryEntities;
using log4net;
using Distributr.Core.Data.EF;
using Distributr.Core.Data.Utility;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Data.Repository.InventoryRepository
{
  internal  class InventoryRepository:IInventoryRepository
    {
      protected static readonly ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      CokeDataContext _ctx;
      ICostCentreRepository _costCentreRepository;
      IProductRepository _productRepository;

      public InventoryRepository(CokeDataContext ctx,ICostCentreRepository costCentreRepository,IProductRepository productRepository)
      {
          _costCentreRepository = costCentreRepository;
          _ctx = ctx;
          _productRepository = productRepository;
      }
      public Inventory GetById(Guid id)
        {
         
            _log.DebugFormat("Getting by id:{0}",id);
            tblInventory tblInv = _ctx.tblInventory.FirstOrDefault(n=>n.id==id);
            if (tblInv == null)
            {
                return null;
            }
            Inventory inv = Map(tblInv);
            return inv;
        }

       public List<Inventory> GetByWareHouseId(Guid id)
        {
            _log.DebugFormat("Getting All By WareHouse Id:{0}",id);
            List<Inventory> qry = _ctx.tblInventory.Where(n => n.WareHouseId == id && n.tblProduct.IM_Status == (int)EntityStatus.Active).ToList().Select(n => Map(n)).ToList();
            return qry;
        }

      public Inventory GetByProductIdAndWarehouseId(Guid id, Guid warehouseId)
      {
         return _ctx.tblInventory.Where(n => n.WareHouseId == warehouseId && n.ProductId==id).ToList().Select(n => Map(n)).FirstOrDefault();
      }

      public List<Inventory> GetAll()
        {
            return _ctx.tblInventory.ToList().Select(n => Map(n)).ToList();

        }
        public Guid AddInventory(Inventory inventory)
        {
            ValidationResultInfo vri=Validate(inventory);
            if (!vri.IsValid)
            {
                
                throw new DomainValidationException(vri,"Failed to validate Inventory");
            }
            tblInventory tblInv = _ctx.tblInventory.FirstOrDefault(n => n.id == inventory.Id); ;
            DateTime dt = DateTime.Now;
            if (tblInv == null)
            {
                tblInv = new tblInventory();
                tblInv.id = inventory.Id;
                tblInv.IM_DateCreated = dt;
                tblInv.IM_Status = (int)EntityStatus.Active;// true;
                _ctx.tblInventory.AddObject(tblInv);
            }
            var entityStatus = (inventory._Status == EntityStatus.New) ? EntityStatus.Active : inventory._Status;
            if (tblInv.IM_Status != (int)entityStatus)
                tblInv.IM_Status = (int)inventory._Status;
            tblInv.IM_DateLastUpdated = dt;
            tblInv.ProductId = inventory.Product.Id;
            tblInv.WareHouseId = inventory.Warehouse.Id;
            tblInv.Balance = inventory.Balance;
            tblInv.UnavailableBalance = inventory.UnavailableBalance;
            tblInv.Value = inventory.Value;
            _ctx.SaveChanges();
            return tblInv.id;
        }

        public Guid UpdateFromServer(Inventory inventory)
        {
            ValidationResultInfo vri = Validate(inventory);
            if (!vri.IsValid)
            {
                throw new DomainValidationException(vri, "Failed to validate Inventory");
            }
            
            tblInventory tblInv = _ctx.tblInventory.FirstOrDefault(n => n.ProductId == inventory.Product.Id && n.WareHouseId == inventory.Warehouse.Id); ;
            DateTime dt = DateTime.Now;
            if (tblInv == null)
            {
                tblInv = new tblInventory();
                tblInv.id = inventory.Id;
                tblInv.IM_DateCreated = dt;
                tblInv.IM_Status = (int)EntityStatus.Active;// true;
                _ctx.tblInventory.AddObject(tblInv);
            }
            var entityStatus = (inventory._Status == EntityStatus.New) ? EntityStatus.Active : inventory._Status;
            if (tblInv.IM_Status != (int)entityStatus)
                tblInv.IM_Status = (int)inventory._Status;
            tblInv.IM_DateLastUpdated = dt;
            tblInv.ProductId = inventory.Product.Id;
            tblInv.WareHouseId = inventory.Warehouse.Id;
            tblInv.Balance = inventory.Balance;
            tblInv.UnavailableBalance = inventory.UnavailableBalance;
            tblInv.Value = inventory.Value;
            _ctx.SaveChanges();
            return tblInv.id;
        }

        public List<Inventory> GetByProductId(Guid id)
        {
            _log.DebugFormat("Getting by Product Id:{0}",id);
            var tblInv = _ctx.tblInventory.Where(n=>n.ProductId==id).ToList();
            if (tblInv == null)
            {
                return null;
            }
            List<Inventory> inv = tblInv.Select(n =>  Map(n)).ToList();
            return inv;
        }

        protected Inventory Map(tblInventory tblInv)
        {
            Inventory inv = new Inventory(tblInv.id)
                                {
                                    Warehouse = _costCentreRepository.GetById(tblInv.tblCostCentre.Id) as Warehouse,
                                    Product = _productRepository.GetById(tblInv.ProductId),
                                    Balance = tblInv.Balance.Value,
                                    Value = (decimal) tblInv.Value,
                                    UnavailableBalance = tblInv.UnavailableBalance,
                                };
            return inv;
        }
        public ValidationResultInfo Validate(Inventory itemToValidate)
        {
            ValidationResultInfo vri = itemToValidate.BasicValidation();
            if (itemToValidate._Status == EntityStatus.Inactive || itemToValidate._Status == EntityStatus.Deleted)
                return vri;
            if (itemToValidate.Id == Guid.Empty)
                vri.Results.Add(new ValidationResult("Enter Valid  Guid ID"));
            return vri;
        }

        public void AdjustInventoryBalance(Guid inventoryId, decimal qty,int type)
        {
            //Available = 1, UnAvailable = 2, Returns = 3, StockTake = 4, AdjustOnly = 5, OutletStockTake=6
            tblInventory tblInv = _ctx.tblInventory.FirstOrDefault(n => n.id == inventoryId);
            if (tblInv == null)
            {
                return ;
            }

            if (type == 1 || type==5)
                tblInv.Balance += qty;
            else if (type == 2)
                tblInv.UnavailableBalance += qty;
            else if (type == 3)
            {
                tblInv.Balance += qty;
                tblInv.UnavailableBalance += (-qty);
            }
            _ctx.SaveChanges();
        }

      public List<Inventory> Query(Guid? warehouseId)
      {
          var query = _ctx.tblInventory.Where(s=>s.tblProduct.IM_Status==(int)EntityStatus.Active).Where(s=>s.Balance>0).AsQueryable();
          if(warehouseId.HasValue )
          {
              query = query.Where(p => p.WareHouseId == warehouseId.Value);
          }

      return query.ToList().Select(n => Map(n)).ToList();
      }
    }
}
