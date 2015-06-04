using System;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands
{
     public class StoredReceivedDeliveryLineItemCommand : ApproveCommand
    {
        public StoredReceivedDeliveryLineItemCommand()
        {
        }

        public StoredReceivedDeliveryLineItemCommand(
            Guid commandId,
            Guid documentId,
             Guid documentItemLineId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            Guid parentDocId,
          
            double? longitude = null, double? latitude = null)
            : base(commandId, documentId, commandGeneratedByUserId, commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId, commandGeneratedByCostCentreApplicationId, parentDocId, longitude, latitude)
        {
           
            DocumentLineItemId = documentItemLineId;
           
        }
        public Guid DocumentLineItemId { get; set; }
       
        public override string CommandTypeRef
        {
            get { return CommandType.StoredReceivedDeliveryLineItem.ToString(); }
        }
    }
}
