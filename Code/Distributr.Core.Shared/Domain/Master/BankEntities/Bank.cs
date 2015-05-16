using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Distributr.Core.Domain.Master.BankEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public class Bank:MasterEntity
   {
       public Bank() : base(default(Guid)) { }
       public Bank(Guid id) : base(id) { }
       public Bank(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive){}
       [Required(ErrorMessage="Name is a Required Field!")]
       public string Name { get; set; }
        [Required(ErrorMessage = "Code is a Required Field!")]
       public string Code { get; set; }
       public string Description { get; set; }
      
   }
}
