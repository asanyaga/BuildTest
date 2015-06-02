using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityTransferCommands
{
    public class ConfirmCommodityTransferCommand : ConfirmCommand
    {
        public ConfirmCommodityTransferCommand()
        {
        }

        public ConfirmCommodityTransferCommand(Guid commandId, 
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
            get { return CommandType.ConfirmCommodityTransfer.ToString(); }
        }
    }
}
