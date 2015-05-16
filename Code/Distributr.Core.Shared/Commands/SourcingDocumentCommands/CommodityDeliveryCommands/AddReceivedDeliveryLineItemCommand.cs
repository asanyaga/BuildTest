using System;
using Distributr.Core.Commands.DocumentCommands;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities.LineItems;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands
{
    public class AddReceivedDeliveryLineItemCommand : AfterCreateCommand
    {
        public AddReceivedDeliveryLineItemCommand()
        {
        }

        public AddReceivedDeliveryLineItemCommand(
            Guid commandId,
            Guid documentId,
           Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
           Guid commodityGradeId,
            Guid parentDocId,
          string containerNo,
           decimal weight,decimal deliveredWeight,SourcingLineItemStatus sourcingLineItemStatus,
           string description,
           double? longitude = null, double? latitude = null)
            : base(commandId, documentId, commandGeneratedByUserId, commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId, commandGeneratedByCostCentreApplicationId,parentDocId,longitude, latitude)
        {
           
            CommodityGradeId = commodityGradeId;
            Weight = weight;
            Description = description;
            ContainerNo = containerNo;
            DeliveredWeight = deliveredWeight;
            LineItemStatus = sourcingLineItemStatus;
        }
        public Guid CommodityId { get; set; }
        public Guid CommodityGradeId { get; set; }
        public Guid ContainerTypeId { get; set; }
       public decimal Weight { get; set; }
        public decimal DeliveredWeight { get; set; }
       public string ContainerNo { get; set; }
        public SourcingLineItemStatus LineItemStatus { get; set; }
       public override string CommandTypeRef
        {
            get { return CommandType.AddReceivedDeliveryLineItem.ToString(); }
        }
    }
}
