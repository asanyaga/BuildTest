using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityPurchaseCommands
{
   public class AddCommodityPurchaseLineItemCommand: AfterCreateCommand
    {
       public AddCommodityPurchaseLineItemCommand(
           Guid commandId, 
           Guid documentId,
            Guid documentItemLineId,
           Guid commandGeneratedByUserId,
           Guid commandGeneratedByCostCentreId,
           int costCentreApplicationCommandSequenceId, 
           Guid commandGeneratedByCostCentreApplicationId,
           Guid parentDocId,
           Guid parentLineItemId,
           Guid commodityId,
           Guid commodityGradeId,
           Guid containerTypeId,
           string containerNo,
           decimal weight,
          
           string description,
           string note,
           decimal tareWeight,
           decimal noofContainers,
           double? longitude =null, 
           double? latitude = null) : base(commandId, documentId, commandGeneratedByUserId, commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId, commandGeneratedByCostCentreApplicationId, parentDocId, longitude, latitude)
       {
           CommodityId = commodityId;
           CommodityGradeId = commodityGradeId;
           ContainerTypeId = containerTypeId;
           Weight = weight;
          
           Description = description;
           Note = note;
           DocumentLineItemId = documentItemLineId;
           ParentLineItemId = parentLineItemId;
           ContainerNo = containerNo;
           TareWeight = tareWeight;
           NoOfContainers = noofContainers;
       }
       public Guid DocumentLineItemId { get; set; }
       public Guid CommodityId { get; set; }
       public Guid CommodityGradeId { get; set; }
       public Guid ContainerTypeId { get; set; }
       public Guid ParentLineItemId { get; set; }
       public string ContainerNo { get; set; }
       public decimal Weight { get; set; }
       public decimal TareWeight { get; set; }
       public decimal NoOfContainers { get; set; }
      
       public string Note { get; set; }
       public override string CommandTypeRef
       {
           get { return CommandType.AddCommodityPurchaseLineItem.ToString(); }
       }

       public AddCommodityPurchaseLineItemCommand()
       {
       }
    }
}
