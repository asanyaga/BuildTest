using System;
using Distributr.Core.Domain.Master.CommodityEntities;
using Distributr.Core.Domain.Master.FarmActivities;

namespace Distributr.Core.Domain.Transactional.ActivityDocumentEntities
{
    public class ActivityProduceItem : TransactionalEntity
    {
        private ActivityProduceItem(Guid id)
            : base(id)
        {
        }

        public Commodity Commodity { get; set; }
        public CommodityGrade Grade { get; set; }
        public decimal Weight { get; set; }
        public string Description { get; set; }
        public ServiceProvider ServiceProvider { get; set; }
    }
}