using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class Transporter : InventoryInTransitWarehouse
    {
       // private int p;

       internal  Transporter(Guid id) : base(id)
        {

        }

        public Transporter(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }

        [Required(ErrorMessage = "DriverName Is Required")]
        public string DriverName { get; set; }
        [Required(ErrorMessage="VehicleRegistrationNo Is Required")]
        public string VehicleRegistrationNo { get; set; }
    }
}
