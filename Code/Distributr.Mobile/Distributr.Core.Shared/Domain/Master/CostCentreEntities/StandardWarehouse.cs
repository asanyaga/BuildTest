using System;

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public abstract class StandardWarehouse : Warehouse
    {
        internal StandardWarehouse(Guid id)
            : base(id)
        {

        }

        public StandardWarehouse(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        { }

        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string VatRegistrationNo { get; set; }
        
    }
}
