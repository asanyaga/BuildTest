using System;
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
    public class Containment : MasterEntity
    {
        public Containment() : base(default(Guid)) { }

        public Containment(Guid id)
            : base(id)
        {
            
        }

        public Containment(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }

        [Required(ErrorMessage = "Containment quantity is a required field")]
        public int Quantity { get; set; }

#if __MOBILE__
        [Ignore]
#endif
        [Required(ErrorMessage = "Returnable product is a required field")]
        public ProductRef ProductRef { get; set; }
    #if __MOBILE__
        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        [Required(ErrorMessage = "Product Packaging type is a required field")]
        public ProductPackagingType ProductPackagingType { get; set; }
    }
}
