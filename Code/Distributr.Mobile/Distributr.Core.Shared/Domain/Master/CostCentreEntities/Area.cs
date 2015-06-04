using System;
using System.ComponentModel.DataAnnotations;
#if __MOBILE__
using SQLiteNetExtensions.Attributes;
#endif

namespace Distributr.Core.Domain.Master.CostCentreEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class Area : MasterEntity
    {
        public Area() : base(default(Guid)) { }
        public Area(Guid id) : base(id)
        {
            
        }
        public Area(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }

        [Required(ErrorMessage = "Area name is a required field")]
        public string Name { get; set; }

        public string Description { get; set; }
    #if __MOBILE__
       [ForeignKey(typeof(Region))]
        public Guid RegionMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        [Required(ErrorMessage = "Region is a required field")]
        public Region region { get; set; }
    }
}
