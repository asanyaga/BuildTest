using System;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master.SuppliersEntities;
#if __MOBILE__
using SQLiteNetExtensions.Attributes;
#endif


namespace Distributr.Core.Domain.Master.ProductEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class ProductBrand : MasterEntity
    {
       public ProductBrand() : base(default(Guid)) { }

        public ProductBrand(Guid id) : base(id)
        {

        }

        public ProductBrand(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }

        [Required(ErrorMessage="Code Is Required")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Name  Is Required")]
       
        public string Name { get; set; }
      
        public string Description { get; set; }
    #if __MOBILE__
        [ForeignKey(typeof(Supplier))]
        public Guid SupplierMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        [Required(ErrorMessage = "The Supplier field is required ")]
        public Supplier Supplier { get; set; }
    }
}
