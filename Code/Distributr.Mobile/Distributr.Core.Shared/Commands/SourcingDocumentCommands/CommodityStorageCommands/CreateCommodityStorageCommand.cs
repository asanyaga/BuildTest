using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityStorageCommands
{
    public class CreateCommodityStorageCommand : CreateCommand
    {
        public CreateCommodityStorageCommand()
        {
        }

        public CreateCommodityStorageCommand
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
           double? longitude = null,
           double? latitude = null) : base(commandId, documentId, commandGeneratedByUserId, 
            commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId, 
            commandGeneratedByCostCentreApplicationId, parentDocId, 
            documentDate,documentIssuerCostCentreid, documentIssuerUserId,
            docRef,
            longitude, latitude)
       {
           
           DateCreated = dateCreated;
           DocumentRecipientCostCentreId = documentRecipientCostCentreId;
           Note = note;
           Description = description;
       }
     
       public DateTime DateCreated { get; set; }

       public Guid DocumentRecipientCostCentreId { get; set; }
       public string Note { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.CreateCommodityStorage.ToString(); }
        }

    }
}
