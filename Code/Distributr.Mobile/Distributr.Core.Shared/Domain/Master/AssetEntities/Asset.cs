using System;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master.AssetEntities;
#if __MOBILE__
using SQLiteNetExtensions.Attributes;
#endif


namespace Distributr.Core.Domain.Master.CoolerEntities
{
#if !SILVERLIGHT
   [Serializable]
#endif
    public class Asset : MasterEntity
    {
        public Asset() : base(default(Guid)) { }
        public Asset(Guid id) : base(id) { }
        public Asset(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
           : base(id, dateCreated, dateLastUpdated, isActive){}


    #if __MOBILE__
        [ForeignKey(typeof(AssetType))]
        public Guid AssetTypeMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        [Required(ErrorMessage = "AssetType is a Required Field!")]
        public AssetType AssetType { get; set; }

    #if __MOBILE__
        [ForeignKey(typeof(AssetCategory))]
        public Guid AssetCategoryMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        [Required(ErrorMessage = "AssetCategory is a Required Field!")]
        public AssetCategory AssetCategory { get; set; }

    #if __MOBILE__
        [ForeignKey(typeof(AssetStatus))]
        public Guid AssetStatusMasterId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
    #endif
        [Required(ErrorMessage = "AssetStatus is a Required Field!")]
        public AssetStatus AssetStatus { get; set; }

        [Required(ErrorMessage = "Code is a Required Field!")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Name is a Required Field!")]
        public string Name { get; set; }
        //[Required(ErrorMessage = "Capacity is a Required Field!")]/*Issues 2626: No required field*/
        public string Capacity { get; set; }
        [Required(ErrorMessage = "Serial Number is a Required Field!")]
        public string SerialNo { get; set; }
        [Required(ErrorMessage = "Asset Number is a Required Field!")]
        public string AssetNo { get; set; }
        
    }
}
