using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Domain.Master.CommodityEntities;

namespace Distributr.Core.Domain.Transactional.SourcingDocumentEnitities
{
    public class ReceivedDelivery
    {
        public Guid Id { get; set; }
        public Guid DocumentId { get; set; }
        public string ContainerNo { get; set; }
        public decimal DeliveredWeight { get; set; }
        public decimal Weight { get; set; }
        public CommodityGrade Grade { get; set; }
        public CommodityGrade DeliveredGrade { get; set; }
    }
}
