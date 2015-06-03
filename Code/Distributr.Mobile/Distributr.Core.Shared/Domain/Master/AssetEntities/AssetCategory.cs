using System;
#if __MOBILE__
using SQLiteNetExtensions.Attributes;
#endif

namespace Distributr.Core.Domain.Master.CoolerEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class AssetCategory:MasterEntity
    {

       public AssetCategory() : base(default(Guid)) { }

        public AssetCategory(Guid id) : base(id)
        {
        }

        public AssetCategory(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {
        }

        public string Name { get; set; }
        public string Description { get; set; }
    #if __MOBILE__
        [ForeignKey(typeof(AssetType))]
        public Guid AssetTypeMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        public AssetType AssetType { get; set; }

    }
}
