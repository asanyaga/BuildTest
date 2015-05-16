using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Master.EquipmentEntities;

namespace Distributr.HQ.Lib.ViewModels.Agrimanagr.EquipmentViewModel
{
   public class VehicleViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Make { get; set; }
        [StringLength(15, MinimumLength = 1, ErrorMessage = "Code length should not exceed 15 charachers.")]
        public string Code { get; set; }
        public string Description { get; set; }
        public string Model { get; set; }
        public string RegistrationNumber { get; set; }
        public Guid HubId { get; set; }
        public string Hub { get; set; }
        public EntityStatus Status { get; set; }
       
    }
}
