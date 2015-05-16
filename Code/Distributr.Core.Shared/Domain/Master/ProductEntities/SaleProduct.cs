using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#if __MOBILE__
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
#endif


namespace Distributr.Core.Domain.Master.ProductEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class SaleProduct : Product
    {
       public SaleProduct() : base(default(Guid)) { }

      public SaleProduct(Guid id) : base(id) { }
        public SaleProduct(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive, List<ProductPricing> productPricings)
            : base(id, dateCreated, dateLastUpdated, isActive, productPricings)
        {

        }

    #if __MOBILE__
        [Column("ContainerCapacity")]    
        public int Capacity { get; set; }

        public Guid ReturnableContainerMasterId { get; set; }
    #endif

    #if __MOBILE__
        public Guid ProductTypeMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif        
        [Required(ErrorMessage = "Product Type Is Required")]
        public ProductType ProductType { get; set; }


    #if __MOBILE__
        public Guid ReturnableProductMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        //Added by Sam
       public ReturnableProduct ReturnableProduct { get; set; }
       // public ReturnableProduct ReturnableProduct { get; set; }
       
        //public decimal UnitCases { get; set; }
    }
}
