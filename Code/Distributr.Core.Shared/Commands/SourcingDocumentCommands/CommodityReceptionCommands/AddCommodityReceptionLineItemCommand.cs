using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityReceptionCommands
{
    public class AddCommodityReceptionLineItemCommand : AfterCreateCommand
    {
        public AddCommodityReceptionLineItemCommand()
        {
        }

        public AddCommodityReceptionLineItemCommand(
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
            int weighType,
           string description,
           string note,
            double? longitude = null, double? latitude = null)
            : base(commandId, documentId, commandGeneratedByUserId, commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId, commandGeneratedByCostCentreApplicationId, parentDocId, longitude, latitude)
        {
            CommodityId = commodityId;
            CommodityGradeId = commodityGradeId;
            ContainerTypeId = containerTypeId;
            Weight = weight;
            weighType = weighType;
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

        public string Note { get; set; }
        public string ContainerNo { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.AddCommodityReceptionLineItem.ToString(); }
        }
    }
}
