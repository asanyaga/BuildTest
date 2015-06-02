using System;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master.ProductEntities;
#if __MOBILE__
using SQLiteNetExtensions.Attributes;
#endif

namespace Distributr.Core.Domain.Master.DistributorTargetEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class TargetItem : MasterEntity
    {
        public TargetItem() : base(default(Guid)) { }

        public TargetItem(Guid id)
            : base(id)
        {

        }
        public TargetItem(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }
    #if __MOBILE__
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        [Required(ErrorMessage = "Product Is Required")]
        public ProductRef Product { get; set; }
        public decimal Quantity { get; set; }
    #if __MOBILE__
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        [Required(ErrorMessage = "Target Is Required")]
        public Target Target { get; set; }
    }
}
