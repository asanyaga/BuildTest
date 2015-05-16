using System;

namespace Distributr.Core.Domain.Master.AssetEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class AssetStatus:MasterEntity
    {
        public AssetStatus() : base(default(Guid)) { }
        public AssetStatus(Guid id) : base(id)
        {
        }

        public AssetStatus(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
        }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
