using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
  public class Provinces:MasterEntity 
    {
      public Provinces(int id) : base(id)
        {
            
        }
      public Provinces(int id, DateTime dateCreated, DateTime dateLastUpdated, bool isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }

        [Required(ErrorMessage = "Province name is a required field")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Country is a required field")]
        public Country countryId { get; set; }
    }
}
