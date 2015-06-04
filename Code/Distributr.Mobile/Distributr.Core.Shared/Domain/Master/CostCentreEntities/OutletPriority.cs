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
    public class OutletPriority:MasterEntity
    {
       public OutletPriority() : base(default(Guid)) { }
        public OutletPriority(Guid id) : base(id)
        {
        }

        public OutletPriority(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
        }

    #if __MOBILE__
        [ForeignKey(typeof(Outlet))]
        public Guid OutletMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Outlet OutletRef { get; set; }

        [Ignore]
    #endif
        [Required(ErrorMessage = "Outlet is a Required Field!")]
        public CostCentreRef Outlet { get; set; }


        [Required(ErrorMessage = "Route is a Required Field!")]
    #if __MOBILE__
        [ForeignKey(typeof(Route))]
        public Guid RouteMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif           
        public Route Route { get; set; }
        
        public int Priority { get; set; }
        [Required(ErrorMessage = "EffectiveDate is a Required Field!")]
        public DateTime EffectiveDate { get; set; }

    }
}
