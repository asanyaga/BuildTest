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
    public class ProductPackaging : MasterEntity
    {
        public ProductPackaging() : base(default(Guid)) { }
        public ProductPackaging(Guid id) : base(id) 
        {

        }
        public ProductPackaging(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }

        [Required(ErrorMessage="Name is a required field")]
        public string Name { get; set; }
        [Required(ErrorMessage="Please enter description")]
        public string Description { get; set; }

    #if __MOBILE__
        [ForeignKey(typeof(Containment))]
        public Guid ContainmentMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif		
        public Containment Containment { get; set; }

    #if __MOBILE__
        public Guid ReturnableProductMasterId { get; set; }
        [Ignore]
    #endif
        public ProductRef ReturnableProductRef { get; set; }
        public string Code { get; set; }
       
    }
}
