using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Distributr.HQ.Lib.ViewModels.Admin.CostCenter
{
   public class TransporterViewModel
    {
       public Guid Id { get; set; }
       public string Code { get; set; }
       public string Name { get; set; }
       [Required(ErrorMessage = "DriverName Is Required")]
       public string DriverName { get; set; }
       [Required(ErrorMessage = "VehicleRegistrationNo Is Required")]
       public string VehicleRegistrationNo { get; set; }
       /*public Guid CostCentreId { get; set; }
       public string ParentCostCentre { get; set; }*/
       public bool isActive { get; set; }
       [Required(ErrorMessage = "Parent CostCenter is a required field")]
       public Guid ParentCostCentre { get; set; }
       [Required(ErrorMessage = "CostCenter Type is a required field")]
       public int CostCentreType { get; set; }
    }
}
