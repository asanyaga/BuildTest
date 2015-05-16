using System;

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class OutletCategory: MasterEntity
    {

       public OutletCategory() : base(default(Guid)) { }
       public OutletCategory(Guid id): base(id){ }
        public OutletCategory(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }

        public string Name { get; set; }
        public string Code { get; set; }
    }
}
