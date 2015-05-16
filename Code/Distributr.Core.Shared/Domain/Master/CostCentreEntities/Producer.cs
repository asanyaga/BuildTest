using System;


namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class Producer : StandardWarehouse
    {
        public Producer() : base(default(Guid)) { }

        public Producer(Guid id)
            : base(id)
        {
        }
        public Producer(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }

        /*
         * Most of contact stuff can be refactored into a costcentre contact detail
         */

       
    }
}
