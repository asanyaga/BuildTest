using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Domain.Master.FarmActivities
{
  public  class ActivityType:MasterEntity
    {
      public ActivityType(Guid id) : base(id)
      {
      }

      public ActivityType(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus status) : base(id, dateCreated, dateLastUpdated, status)
      {
      }

      [Required(ErrorMessage = "Code is a Required Field")]
      public string Code { get; set; }
      [Required(ErrorMessage = "Name is a Required Field")]
      public string Name { get; set; }
      public string Description { get; set; }
      public bool IsInfectionsRequired { get; set; }
      public bool IsInputRequired { get; set; }
      public bool IsServicesRequired { get; set; }
      public bool IsProduceRequired { get; set; }
    }
}
