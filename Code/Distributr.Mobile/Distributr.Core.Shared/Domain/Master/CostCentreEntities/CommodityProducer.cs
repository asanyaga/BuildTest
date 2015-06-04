using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.Master.CentreEntity;

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class CommodityProducer : MasterEntity
    {
        public CommodityProducer(Guid id)
            : base(id)
        {
            CommodityProducerCentres = new List<Centre>();
        }

        public CommodityProducer(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
        }
        public string Code { get; set; }

        [Required(ErrorMessage = "Acreage is required")]
        public string Acrage { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Registration number is required")]
        public string RegNo { get; set; }

        public string PhysicalAddress { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Commodity Supplier is required")]
        public CommoditySupplier CommoditySupplier { get; set; }

        public List<Centre> CommodityProducerCentres { get; set; }
    }
}
