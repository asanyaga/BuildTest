using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityStorageCommands
{
    public class TransferedCommodityStorageLineItemCommand : DocumentCommand
    {
        public TransferedCommodityStorageLineItemCommand()
        {
        }

        public TransferedCommodityStorageLineItemCommand(
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
            get { return CommandType.TransferedCommodityStorage.ToString(); }
        }
    }
}
