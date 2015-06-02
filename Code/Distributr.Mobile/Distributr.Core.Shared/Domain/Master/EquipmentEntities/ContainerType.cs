using System;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Domain.Master.CommodityEntities;


   namespace Distributr.Core.Domain.Master.EquipmentEntities
   {
#if !SILVERLIGHT
       [Serializable]
#endif
       public class ContainerType:MasterEntity
       {
           public ContainerType(Guid id) : base(id) { }

           public ContainerType(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
               : base(id, dateCreated, dateLastUpdated, isActive) { }
           [Required]
           public string Name { get; set; }
           public string Make { get; set; }
           public string Code { get; set; }
           public string Description { get; set; }
           public string Model { get; set; }
           public decimal LoadCarriage { get; set; }
           public decimal TareWeight { get; set; }
           public decimal Length { get; set; }
           public decimal Width { get; set; }
           public decimal Height { get; set; }
           public decimal BubbleSpace { get; set; }
           public decimal Volume { get; set; }
           public decimal FreezerTemp { get; set; }
           public CommodityGrade CommodityGrade { get; set; }
           public ContainerUseType ContainerUseType { get; set; }
       }
   }