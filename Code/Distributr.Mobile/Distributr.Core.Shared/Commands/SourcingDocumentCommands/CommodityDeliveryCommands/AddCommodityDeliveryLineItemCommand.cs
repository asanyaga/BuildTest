using System;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands
{
    public class AddCommodityDeliveryLineItemCommand : AfterCreateCommand
    {
        public AddCommodityDeliveryLineItemCommand()
        {
        }

        public AddCommodityDeliveryLineItemCommand(
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
            int weightype,
           string description,
           string note,
            double? longitude = null, double? latitude = null)
            : base(commandId, documentId, commandGeneratedByUserId, commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId, commandGeneratedByCostCentreApplicationId, parentDocId, longitude, latitude)
        {
            CommodityId = commodityId;
            CommodityGradeId = commodityGradeId;
            ContainerTypeId = containerTypeId;
            Weight = weight;
            WeighType = weightype;
            
            Description = description;
            Note = note;
            DocumentLineItemId = documentItemLineId;
            ParentLineItemId = parentLineItemId;
            ContainerNo = containerNo;
        }
        public Guid DocumentLineItemId { get; set; }
        public Guid CommodityId { get; set; }
        public Guid CommodityGradeId { get; set; }
        public Guid ContainerTypeId { get; set; }
        public Guid ParentLineItemId { get; set; }
        public decimal Weight { get; set; }
        public int WeighType { get; set; }
        public decimal NoOfContainers { get; set; }
        public string Note { get; set; }
        public string ContainerNo { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.AddCommodityDeliveryLineItem.ToString(); }
        }
    }
}
