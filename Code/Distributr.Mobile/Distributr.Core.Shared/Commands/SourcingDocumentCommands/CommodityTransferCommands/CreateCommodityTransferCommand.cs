using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityTransferCommands
{
    public class CreateCommodityTransferCommand : CreateCommand
    {
        public CreateCommodityTransferCommand()
        {
        }

        public CreateCommodityTransferCommand
           (Guid commandId,
           Guid documentId,
           Guid commandGeneratedByUserId,
           Guid commandGeneratedByCostCentreId,
           int costCentreApplicationCommandSequenceId,
           Guid commandGeneratedByCostCentreApplicationId,
           Guid parentDocId,
           Guid documentRecipientCostCentreId,
           string note,
           string docRef,
           string description,
           DateTime dateCreated,
           DateTime documentDate,
           Guid documentIssuerCostCentreid,
           Guid documentIssuerUserId,
           int documentTypeId2,
           Guid? wareHouseToStore = null,
           double? longitude = null,
           double? latitude = null)
            : base(commandId, documentId, commandGeneratedByUserId,
                commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId,
                commandGeneratedByCostCentreApplicationId, parentDocId,
                documentDate, documentIssuerCostCentreid, documentIssuerUserId,
                docRef,
                longitude, latitude)
        {
            DateCreated = dateCreated;
            DocumentRecipientCostCentreId = documentRecipientCostCentreId;
            Note = note;
            Description = description;
            WareHouseToStore = wareHouseToStore;
            DocumentTypeId2 = documentTypeId2;
        }

        public DateTime DateCreated { get; set; }
        public Guid DocumentRecipientCostCentreId { get; set; }
        public string Note { get; set; }
        public Guid? WareHouseToStore { get; set; }
        public int DocumentTypeId2 { get; set; }

        public override string CommandTypeRef
        {
            get { return CommandType.CreateCommodityTransfer.ToString(); }
        }
    }
}
