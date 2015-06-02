using System;
using System.ComponentModel.DataAnnotations;

namespace Distributr.Core.Domain.Master.ProductEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public class DiscountGroup:MasterEntity
    {
       public DiscountGroup() : base(default(Guid)) { }

       public DiscountGroup(Guid id)
           : base(id)
       { }
       public DiscountGroup(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive)
       { 
       }
       [Required(ErrorMessage = "Code is Required")]
       public string Code { get; set; }

       [Required(ErrorMessage = "Name is Required")]
       public string Name { get; set; }
    }
}
