using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CostCentreEntities;

namespace Distributr.Core.Domain.Master.FarmActivities
{
    public class Season:MasterEntity
    {
        public Season(Guid id) : base(id)
        {
        }

        public Season(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus status) : base(id, dateCreated, dateLastUpdated, status)
        {
        }

        [StringLength(50),Required(ErrorMessage="Code is a Required Field")]
        public string Code { get; set; }

        [StringLength(50), Required(ErrorMessage = "Name is a Required Field")]
        public string Name { get; set; }

        [Required(ErrorMessage="Commodity Proudcer is a required field")]
        public CommodityProducer CommodityProducer { get; set; }

        [StringLength(450)]
        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
