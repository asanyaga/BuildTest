using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#if __MOBILE__ 
using SQLiteNetExtensions.Attributes;
#endif

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

    #if __MOBILE__ 
       [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
       public List<BankBranch> Branches { get; set; }
    #endif
   }
}
