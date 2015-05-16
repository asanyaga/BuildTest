using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityReceptionCommands
{
   public  class CreateCommodityReceptionCommand: CreateCommand
    {
       public CreateCommodityReceptionCommand()
       {
       }

       public CreateCommodityReceptionCommand
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
           Guid documentIssuerCostCentreId,
           Guid documentIssuedUserId,
            DateTime? vehicleArrivalTime = null,
       DateTime? vehicleDepartureTime = null,
        decimal? vehicleArrivalMileage = null,
       decimal? vehicleDepartureMileage = null,
           double? longitude = null,
           double? latitude = null) : base(commandId, documentId, commandGeneratedByUserId, 
           commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId,
           commandGeneratedByCostCentreApplicationId, parentDocId,
           documentDate,
           documentIssuerCostCentreId,
           documentIssuedUserId,
           docRef,
           longitude, latitude)
       {
           DateCreated = dateCreated;
           DocumentRecipientCostCentreId = documentRecipientCostCentreId;
           Note = note;
           Description = description;
           VehicleArrivalMileage = vehicleArrivalMileage;
           VehicleArrivalTime = vehicleArrivalTime;
           VehicleDepartureMileage = vehicleDepartureMileage;
           VehicleDepartureTime = vehicleDepartureTime;
       }
       public string Description { get; set; }
       public DateTime DateCreated { get; set; }

       public Guid DocumentRecipientCostCentreId { get; set; }
       public string Note { get; set; }

       public DateTime? VehicleArrivalTime { get; set; }
       public DateTime? VehicleDepartureTime { get; set; }

       public decimal? VehicleArrivalMileage { get; set; }
       public decimal? VehicleDepartureMileage { get; set; }

       public override string CommandTypeRef
       {
           get { return CommandType.CreateCommodityReception.ToString(); }
       }
    }
}
