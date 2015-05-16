using System;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands
{
    public class ConfirmCommodityDeliveryCommand : ConfirmCommand
    {
        public ConfirmCommodityDeliveryCommand()
        {
        }

        public ConfirmCommodityDeliveryCommand(Guid commandId, 
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId, 
            Guid parentDocId,
            double? longitude = null, double? latitude = null) : base(commandId, documentId, commandGeneratedByUserId, commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId, commandGeneratedByCostCentreApplicationId, parentDocId, longitude, latitude)
        {
        }

        public override string CommandTypeRef
        {
            get { return CommandType.ConfirmCommodityDelivery.ToString(); }
        }

    }
}
