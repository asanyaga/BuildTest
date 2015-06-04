using System;
using System.ComponentModel.DataAnnotations;

namespace Distributr.Core.Domain.Master.ProductEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class ProductPackagingType : MasterEntity
    {
       public ProductPackagingType() : base(default(Guid)) { }

        public ProductPackagingType(Guid id) : base(id)
        {

        }
        public ProductPackagingType(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }
        [Required(ErrorMessage="Name is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        public string Code { get; set; }
        
    }
}
