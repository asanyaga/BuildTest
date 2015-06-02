using System;
using System.ComponentModel.DataAnnotations;
#if __MOBILE__
using SQLiteNetExtensions.Attributes;
#endif

namespace Distributr.Core.Domain.Master.ProductEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class ProductFlavour : MasterEntity
    {
          public ProductFlavour() : base(default(Guid)) { }
        public ProductFlavour(Guid id) : base(id)
        {

        }
        public ProductFlavour(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }
        [Required(ErrorMessage="Code is Required")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is Required")]
        public string Description { get; set; }

    #if __MOBILE__
        [ForeignKey(typeof(ProductBrand))]
        public Guid ProductBrandMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public ProductBrand ProductBrand { get; set; }

    }
}
