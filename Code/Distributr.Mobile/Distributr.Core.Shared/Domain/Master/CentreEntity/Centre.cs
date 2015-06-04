using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.Core.Domain.Master.CentreEntity
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class Centre : MasterEntity
    {
        public Centre(Guid id) : base(id)
        {
        }

        public Centre(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
        }

        public string Code { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Centre Type is required")]
        public CentreType CenterType { get; set; }

        [Required(ErrorMessage = "Hub is required")]
        public Hub Hub { get; set; }

        [Required(ErrorMessage = "Route is required")]
        public Route Route { get; set; }

    }
}
