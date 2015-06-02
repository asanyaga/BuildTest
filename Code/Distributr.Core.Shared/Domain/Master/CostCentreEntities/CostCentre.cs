using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#if __MOBILE__
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
#endif

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
    public class CostCentreDetailRef
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string CostCentreType { get; set; }
    }
#if !SILVERLIGHT
   [Serializable]
#endif
    public class CostCentreRef
    {
        public Guid Id { get; set; }

    }
   public class CostCentreMapping
   {
       public Guid Id { get; set; }
       public Guid MappToId { get; set; }

   }

#if !SILVERLIGHT
   [Serializable]
#endif
    public class CostCentre : MasterEntity
    {
       public CostCentre() : base(default(Guid))
       {
       }

       internal CostCentre(Guid id) : base(id)
        {
            Contact = new List<Contact>();
        }

        public CostCentre(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
            Contact = new List<Contact>();
        }
        
        [Required(ErrorMessage="Name is required")]
        public  string Name { get; set; }
        public string CostCentreCode { get; set; }
    #if __MOBILE__        
       [Ignore]
    #endif
        public List<Contact> Contact { get; set; }

    #if __MOBILE__
        public Guid ParentCostCentreId { get; set; }
    #endif

    #if __MOBILE__
           [Ignore]
    #endif
        public CostCentreRef ParentCostCentre { get;  set; }
    
    
    #if __MOBILE__
           [Column("CostCentreTypeId")]
    #endif    
       public CostCentreType CostCentreType { get;  set; }              
    }
}
