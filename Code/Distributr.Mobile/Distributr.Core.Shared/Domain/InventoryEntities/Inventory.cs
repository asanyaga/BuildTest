using System;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.ProductEntities;
#if __MOBILE__
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
#endif

namespace Distributr.Core.Domain.InventoryEntities
{
    public class Inventory : MasterEntity
    {
        public Inventory() : base(default(Guid)) { }
        public Inventory(Guid id)
            : base(id)
        { }
        public Inventory(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        { }
    #if __MOBILE__
        //[ForeignKey(typeof(Warehouse))]
        public Guid WarehouseMasterID { get; set; }

        [Ignore]
    #endif     
        public Warehouse Warehouse { get; set; }


    #if __MOBILE__
        public Guid ProductMasterID { get; set; }

        [Ignore]
    #endif
        public Product Product { get; set; }
        public decimal Balance { get; set; }
        public decimal UnavailableBalance { get; set; }
        public decimal Value { get; set; }
    }
    public class SourcingInventory : MasterEntity
    {
        public SourcingInventory() : base(default(Guid)) { }
        public SourcingInventory(Guid id)
            : base(id)
        { }
        public SourcingInventory(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        { }
    #if __MOBILE__
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif		
        public Warehouse Warehouse { get; set; }
    #if __MOBILE__
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif		
        public Commodity Commodity { get; set; }
    #if __MOBILE__
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif		
        public CommodityGrade Grade { get; set; }
        public decimal Balance { get; set; }
        public decimal UnavailableBalance { get; set; }
        public decimal Value { get; set; }
    }
}
