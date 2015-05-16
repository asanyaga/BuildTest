using System;
using Distributr.Core.Domain.Master.FarmActivities;

namespace Distributr.Core.Domain.Transactional.ActivityDocumentEntities
{
    public class ActivityInfectionItem : TransactionalEntity
    {
        private ActivityInfectionItem(Guid id)
            : base(id)
        {
        }
        public Infection  Infection{ get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }
    }
}