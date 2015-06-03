using System.Collections.Generic;
using System.Collections;
using System;
using System.ComponentModel.DataAnnotations;
#if __MOBILE__
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
#endif

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class Route : MasterEntity
    {

        public Route() : base(default(Guid)) { }

        public Route(Guid id) : base(id) { }

        public Route(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }

        [Required(ErrorMessage = "Name Is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Code Is Required")]
        public string Code { get; set; }
    #if __MOBILE__
        [ForeignKey(typeof(Region))]
        [Column("RegionId")]
        public Guid RegionMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        [Required(ErrorMessage = "Region is required")]
        public Region Region { get; set; }
    
    #if __MOBILE__
        [OneToMany(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
       public Outlet[] Outlets { get; set; }
    #endif

    }
}
