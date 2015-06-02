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
   public  class OutletVisitDay:MasterEntity
    {
       public OutletVisitDay() : base(default(Guid)) { }

       public OutletVisitDay(Guid id) : base(id)
       {

       }

       public OutletVisitDay(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
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
       public DayOfWeek Day { get; set; }
       [Required(ErrorMessage = "EffectiveDate is a Required Field!")]
       public DateTime EffectiveDate { get; set; }
    }
}
