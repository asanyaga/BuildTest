using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands
{
   public abstract  class CreateCommand:DocumentCommand
    {
       public CreateCommand()
       {
           
       }
       protected CreateCommand(Guid commandId,
           Guid documentId, 
           Guid commandGeneratedByUserId, 
           Guid commandGeneratedByCostCentreId, 
           int costCentreApplicationCommandSequenceId,
           Guid commandGeneratedByCostCentreApplicationId, 
           Guid parentDocId, 
           DateTime documentDateIssued,
           Guid documentIssuerCostCentreId,
           Guid docIssuerUserId,
           string documentReference,
           double? longitude = null, double? latitude = null) :
           base(commandId, documentId, commandGeneratedByUserId, commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId,
           commandGeneratedByCostCentreApplicationId, parentDocId, longitude, latitude)
       {
           DocumentDateIssued = documentDateIssued;
           DocumentIssuerCostCentreId = documentIssuerCostCentreId;
           DocIssuerUserId = docIssuerUserId;
           DocumentReference = documentReference;
           
       }

       public DateTime DocumentDateIssued { get; set; }
       public Guid DocumentIssuerCostCentreId { get; set; }
       public Guid DocIssuerUserId { get; set; }
       public string DocumentReference { get; set; }
       public string ExtDocumentReference { get; set; }
       public string VersionNumber { get; set; }
       
    }
}
