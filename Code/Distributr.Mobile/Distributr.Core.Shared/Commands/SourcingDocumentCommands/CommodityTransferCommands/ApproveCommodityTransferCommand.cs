using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityTransferCommands
{
    public class ApproveCommodityTransferCommand : DocumentCommand
    {
        public ApproveCommodityTransferCommand()
        {
        }

        public ApproveCommodityTransferCommand(
            Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            Guid parentDocId,
            Guid wareHouseId,
            double? longitude = null, double? latitude = null)
            : base(commandId, documentId, commandGeneratedByUserId, commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId, commandGeneratedByCostCentreApplicationId, parentDocId, longitude, latitude)
        {
            WareHouseId = wareHouseId;
        }

        public Guid WareHouseId { get; set; }

        public override string CommandTypeRef
        {
            get { return CommandType.ApproveCommodityTransfer.ToString(); }
        }
    }
}
