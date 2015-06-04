using System;
#if __MOBILE__
using SQLite.Net.Attributes;
#endif

namespace Distributr.Core.Domain.Master
{
    public enum EntityStatus { New = 0, Active = 1, Inactive = 2, Deleted = 3 }
#if !SILVERLIGHT
   [Serializable]
#endif
    public abstract class MasterEntity: IInfrastructureMetadata
    {
       public MasterEntity(Guid id)
        {
            Id = id;
        }

        public MasterEntity(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus status)
            : this(id)
        {
            _DateCreated = dateCreated;
            _DateLastUpdated = dateLastUpdated;
            _Status = status;
        }

       #if __MOBILE__
        [PrimaryKey, Column("MasterId")]
       #endif
       public virtual Guid Id { get; set; }

        #if __MOBILE__
            [Column("DateCreated")]
        #endif
       public DateTime _DateCreated { get; set; }
        internal void _SetDateCreated(DateTime dateCreated)
        {
            _DateCreated = dateCreated;
        }
    #if __MOBILE__
            [Column("DateLastUpdated")]
    #endif        
        public DateTime _DateLastUpdated { get; set; }
        internal void _SetDateLastUpdated(DateTime dateLastUpdated)
        {
            _DateLastUpdated = dateLastUpdated;
        }
    #if __MOBILE__
            [Column("StatusId")]
    #endif        
        public EntityStatus _Status { get; set; }
        internal void _SetStatus(EntityStatus status )
        {
            _Status = status;
        }
               
    }
}