using System;
using System.ComponentModel.DataAnnotations;

namespace Distributr.Core.Domain.Master.SuppliersEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public class Supplier:MasterEntity
    {
       public Supplier() : base(default(Guid)) { }

       public Supplier(Guid id):base(id)
       {

       }
       public Supplier(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           :base(id,dateCreated,dateLastUpdated,isActive)
       {

       }
       [Required]
       public string Name { get; set; }
       [Required]
       public string Code { get; set; }

       public string Description { get; set; }
    }
}
