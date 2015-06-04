using System;
using Distributr.Core.Domain.Master.FarmActivities;

namespace Distributr.Core.Domain.Transactional.ActivityDocumentEntities
{
    public class ActivityServiceItem : TransactionalEntity
    {
        private ActivityServiceItem(Guid id)
            : base(id)
        {
        }

        public CommodityProducerService Service { get; set; }
        public ServiceProvider ServiceProvider { get; set; }
        public Shift Shift { get; set; }
        public string Description { set; get; }
    }
}