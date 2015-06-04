using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Domain.Master.FarmActivities
{
    public class CommodityProducerService:MasterEntity
    {
        public CommodityProducerService(Guid id) : base(id)
        {
        }

        public CommodityProducerService(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus status) : base(id, dateCreated, dateLastUpdated, status)
        {
        }

        [StringLength(50),Required(ErrorMessage="Code is a Required Field")]
        public string Code { get; set; }

        [StringLength(50),Required(ErrorMessage="Name is a Required Field")]
        public string Name { get; set; }


        public decimal Cost { get; set; }

        [StringLength(450)]
        public string Description { get; set; }




    }
}
