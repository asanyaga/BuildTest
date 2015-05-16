using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Value;
using Distributr.Core.Domain.Master.UserEntities;
using System.Web.Mvc;

namespace Distributr.HQ.Lib.ViewModels.Admin.ProducerViewModel
{
    public class AdminProducerViewModel
    {

        public Guid Id { get; set; }
    
        public string Name { get; set; }
        [Required(ErrorMessage = "Password is required")]

        public string Longitude { get; set; }
        public string Latitude { get; set; }
        
      
    }
}
