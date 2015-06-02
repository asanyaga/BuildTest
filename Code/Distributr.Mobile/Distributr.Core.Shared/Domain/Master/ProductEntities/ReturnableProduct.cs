using System;
using System.Collections.Generic;
#if __MOBILE__
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
#endif

namespace Distributr.Core.Domain.Master.ProductEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class ReturnableProduct : Product
    {
        public ReturnableProduct() : base(default(Guid)) { }

       internal ReturnableProduct(Guid id) : base(id) { }
        public ReturnableProduct(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive, List<ProductPricing> productPricings)
            : base(id, dateCreated, dateLastUpdated, isActive, productPricings)
        {

        }
        public int Capacity { get; set; }

    #if __MOBILE__
        [ForeignKey(typeof(ReturnableProduct))]
        public Guid ReturnAbleProductMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public ReturnableProduct ReturnAbleProduct { get; set; }
    }
}
