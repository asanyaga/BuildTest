using System;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands
{
   public  class CreateReceivedDeliveryCommand: CreateCommand
    {
       public CreateReceivedDeliveryCommand()
       {
       }

       public CreateReceivedDeliveryCommand
           (Guid commandId, 
           Guid documentId,
           Guid commandGeneratedByUserId, 
           Guid commandGeneratedByCostCentreId, 
           int costCentreApplicationCommandSequenceId, 
           Guid commandGeneratedByCostCentreApplicationId, 
           Guid parentDocId,
           string docRef,
           string description,
           DateTime dateCreated,
           DateTime documentDate,Guid routeId,Guid centreId,
            DateTime? vehicleArrivalTime = null,
       DateTime? vehicleDepartureTime = null,
        decimal? vehicleArrivalMileage = null,
        decimal? vehicleDepartureMileage = null,
           double? longitude = null,
           double? latitude = null) 
           : base(commandId, documentId, commandGeneratedByUserId, 
           commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId,
           commandGeneratedByCostCentreApplicationId, parentDocId,
           documentDate,
           commandGeneratedByCostCentreId,
           commandGeneratedByUserId,
           docRef,
           longitude, latitude)
       {
           DateCreated = dateCreated;
          Description = description;
          RouteId = routeId;
          CentreId = centreId;
          VehicleArrivalMileage = vehicleArrivalMileage;
          VehicleArrivalTime = vehicleArrivalTime;
          VehicleDepartureMileage = vehicleDepartureMileage;
          VehicleDepartureTime = vehicleDepartureTime;
       }
       public Guid DocumentRecipientCostCentreId { get; set; }
       public Guid RouteId { get; set; }
       public Guid CentreId { get; set; }
    
       public DateTime DateCreated { get; set; }

       public DateTime? VehicleArrivalTime { get; set; }
       public DateTime? VehicleDepartureTime { get; set; }

       public decimal? VehicleArrivalMileage { get; set; }
       public decimal? VehicleDepartureMileage { get; set; }
      
       public override string CommandTypeRef
       {
           get { return CommandType.CreateReceivedDelivery.ToString(); }
       }
    }
}
