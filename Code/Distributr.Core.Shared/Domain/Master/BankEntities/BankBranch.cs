using System;
using System.ComponentModel.DataAnnotations;

#if __MOBILE__
using SQLiteNetExtensions.Attributes;
#endif

namespace Distributr.Core.Domain.Master.BankEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public class BankBranch:MasterEntity 
    {
        public BankBranch() : base(default(Guid)) { }
        public BankBranch(Guid id) : base(id) { }
        public BankBranch(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive){}
        [Required(ErrorMessage = "Name is a Required Field!")]
       public string Name { get; set; }
        [Required(ErrorMessage = "Code is a Required Field!")]
       public string Code { get; set; }
    #if __MOBILE__
       [ForeignKey(typeof(Bank))]
        public Guid BankMasterId { get; set; }
        [ManyToOne()]
    #endif
        [Required(ErrorMessage = "Bank is a Required Field!")]
       public Bank Bank { get; set; }
       public string Description { get; set; }
    }
}
