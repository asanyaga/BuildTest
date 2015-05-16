using System;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityReleaseCommands
{
   public class AddCommodityReleaseNoteLineItemCommand: AfterCreateCommand
    {
  
       public Guid DocumentLineItemId { get; set; }
       public Guid CommodityId { get; set; }
       public Guid CommodityGradeId { get; set; }
       public Guid ContainerTypeId { get; set; }
       public Guid ParentLineItemId { get; set; }
       public string ContainerNo { get; set; }
       public decimal Weight { get; set; }
      
       public string Note { get; set; }
       public override string CommandTypeRef
       {
           get { return CommandType.AddCommodityReleaseNoteLineItem.ToString(); }
       }

       public AddCommodityReleaseNoteLineItemCommand()
       {
       }
    }
}
