using System;
#if __MOBILE__
using SQLite.Net.Attributes;
#endif
using System.ComponentModel.DataAnnotations;

namespace Distributr.Core.Domain.Master.OutletVisitReasonsTypeEntities
{
    public enum OutletVisitAction { NoAction = 0 ,Productive=1,UnProductive =2}
    public class OutletVisitReasonsType:MasterEntity 
    {

        public OutletVisitReasonsType(Guid id) :base (id)
        {}

        public OutletVisitReasonsType (Guid id, DateTime dateCreated, DateTime dateLastUpdated,
            EntityStatus isActive) : base(id ,dateCreated,dateLastUpdated, isActive)
        {
            
        }

        [Required(ErrorMessage = "OutletVisitReasonsType name is a required field")]
        public string Name { get; set; }
       
        public string Description { get; set; }

    #if __MOBILE__
        [Column("OutletVisitActionId")]
    #endif
        public OutletVisitAction OutletVisitAction { get; set; }
    }
}
