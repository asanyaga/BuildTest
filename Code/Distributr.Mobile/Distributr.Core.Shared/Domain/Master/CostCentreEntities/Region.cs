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
    public class Region : MasterEntity
    {
        public Region() : base(default(Guid)) { }
        public Region(Guid id) : base(id)
        {
            
        }
        public Region(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        {

        }
        [Required(ErrorMessage = "Region name is a required field")]
        public string Name { get; set; }

        public string Description { get; set; }

    #if __MOBILE__
        [ForeignKey(typeof(Country))]
        public Guid CountryMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        [Required(ErrorMessage = "Country is a required field")]
        public Country Country { get; set; }
       // [Required(ErrorMessage = "Province is a required field")]
       // public Provinces provinceId { get; set; }
       //[Required(ErrorMessage = "District is a required field")]
       // public District districtId { get; set; }
    }
}