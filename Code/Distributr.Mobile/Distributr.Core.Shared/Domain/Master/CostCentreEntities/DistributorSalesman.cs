using System;
using System.Collections.Generic;
using Distributr.Core.Domain.Master.SuppliersEntities;
#if __MOBILE__
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
#endif

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
    public enum DistributorSalesmanType { Salesman =0,Stockist=1,StockistSalesman=2}
#if !SILVERLIGHT
   [Serializable]
#endif
    public class DistributorSalesman : InventoryInTransitWarehouse
    {
        public DistributorSalesman() : base(default(Guid)) { }

        internal DistributorSalesman(Guid id) : base(id)
        {
            Routes = new List<SalesmanRoute>();
        }

        public DistributorSalesman(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
            Routes = new List<SalesmanRoute>();
        }

    #if __MOBILE__
        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public List<SalesmanRoute> Routes { get; set; }
    
    #if __MOBILE__
       [Column("TypeId")]
    #endif
        public DistributorSalesmanType Type { get; set; }

    #if __MOBILE__
        [ForeignKey(typeof(Route))]
        public Guid RouteMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Route Route { get; set; }
    #endif
        
       
    }
#if !SILVERLIGHT
   [Serializable]
#endif
    public class SalesmanRoute : MasterEntity
    {
        public SalesmanRoute() : base(default(Guid)) { }
        public SalesmanRoute(Guid id) : base(id) { }
        public SalesmanRoute(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
         {
         }
    #if __MOBILE__
        [ForeignKey(typeof(Route))]
        public Guid RouteMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public Route Route { get; set; }

    #if __MOBILE__
        [ForeignKey(typeof(DistributorSalesman))]
        public Guid DistributorSalesmanMasterId { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public DistributorSalesman DistributorSalesman { get; set; }

        [Ignore]
    #endif
        public CostCentreRef DistributorSalesmanRef { get; set; }

    }
#if !SILVERLIGHT
   [Serializable]
#endif
   public class SalesmanSupplier : MasterEntity
   {
       public SalesmanSupplier() : base(default(Guid)) { }
       public SalesmanSupplier(Guid id) : base(id) { }
       public SalesmanSupplier(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive)
       {
       }


    #if __MOBILE__
       [ForeignKey(typeof(Supplier))]
       public Guid SupplierMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
       public Supplier Supplier { get; set; }

    #if __MOBILE__
        [ForeignKey(typeof(DistributorSalesman))]
        public Guid DistributorSalesmanMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public DistributorSalesman DistributorSalesman { get; set; }

        [Ignore]
    #endif
       public CostCentreRef DistributorSalesmanRef { get; set; }

    #if __MOBILE__
       public bool Assigned { get; set; }
    #endif
   }    
}
