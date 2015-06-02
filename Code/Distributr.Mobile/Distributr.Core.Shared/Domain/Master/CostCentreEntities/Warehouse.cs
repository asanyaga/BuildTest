using System;

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public abstract class Warehouse : CostCentre
    {
        internal Warehouse(Guid id)
            : base(id)
        {

        }

        public Warehouse(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        { }

    #if __MOBILE__
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string VatRegistrationNo { get; set; }
    #endif

    }
}
