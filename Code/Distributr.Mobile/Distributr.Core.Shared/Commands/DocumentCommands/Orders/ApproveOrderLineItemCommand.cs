using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributr.Core.Commands.DocumentCommands.Orders
{
   public class ApproveOrderLineItemCommand:AfterConfirmCommand
    {
       public ApproveOrderLineItemCommand()
       {
       }

       public override string CommandTypeRef
       {
           get { return CommandType.ApproveOrderLineItem.ToString(); }
       }

       public ApproveOrderLineItemCommand(Guid commandId,
           Guid documentId, Guid commandGeneratedByUserId, 
           Guid commandGeneratedByCostCentreId, 
           int costCentreApplicationCommandSequenceId, 
           Guid commandGeneratedByCostCentreApplicationId,
           Guid parentDocId, Guid lineItemId,decimal approvedQuantity,
           double? longitude = null, double? latitude = null) : base(commandId, documentId, commandGeneratedByUserId, commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId, commandGeneratedByCostCentreApplicationId, parentDocId, longitude, latitude)
       {
           LineItemId = lineItemId;
           ApprovedQuantity = approvedQuantity;
       }

       public Guid LineItemId { get; set; }
       public decimal ApprovedQuantity { get; set; }
       public decimal LossSaleQuantity { get; set; }
       
    }
}
