using System;
using System.ComponentModel.DataAnnotations;

namespace Distributr.Core.Domain.Master.DistributorTargetEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
   public class TargetPeriod:MasterEntity 
    {
       public TargetPeriod() : base(default(Guid)) { }
       public TargetPeriod(Guid id):base (id)
       {
       }
       public TargetPeriod(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive)
       {
       }

  
      [Required(ErrorMessage="Name is a Required Field!")]
       public string Name { get; set; }
      DateTime _startDate;

      public DateTime StartDate
      {
          get { return  _startDate.Date; }
          set { _startDate = value; }
      }

      DateTime _endDate;

      public DateTime EndDate
      {
          get { return _endDate.Date.AddDays(1).Subtract(new TimeSpan(0,0,3)); }
          set { _endDate = value; }
      }


       public bool IsWithinDateRange(DateTime date)
       {
           return date >= StartDate && date <= EndDate;
       }
    }
}
