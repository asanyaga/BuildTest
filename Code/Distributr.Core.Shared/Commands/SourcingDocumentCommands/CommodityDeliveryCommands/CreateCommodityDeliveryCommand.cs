using System;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands
{
    public  class CreateCommodityDeliveryCommand: CreateCommand
    {
       public CreateCommodityDeliveryCommand()
       {
       }

       public CreateCommodityDeliveryCommand
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
           string driverName,
           string vehicleRegNo, Guid routeId,Guid centreId,
            DateTime? vehicleArrivalTime=null,
       DateTime? vehicleDepartureTime = null,
        decimal? vehicleArrivalMileage = null,
        decimal? vehicleDepartureMileage = null,
           double? longitude = null,
           double? latitude = null) : base(commandId, documentId, commandGeneratedByUserId, 
           commandGeneratedByCostCentreId, costCentreApplicationCommandSequenceId,
           commandGeneratedByCostCentreApplicationId, parentDocId,
           documentDate,
           commandGeneratedByCostCentreId,
           commandGeneratedByUserId,
           docRef,
           longitude, latitude)
       {
           DateCreated = dateCreated;
           DocumentRecipientCostCentreId = documentRecipientCostCentreId;
           Note = note;
           Description = description;
           DriverName = driverName;
           VehicleRegNo = vehicleRegNo;
           RouteId = routeId;
           CentreId = centreId;
           VehicleArrivalMileage = vehicleArrivalMileage;
           VehicleArrivalTime = vehicleArrivalTime;
           VehicleDepartureMileage = vehicleDepartureMileage;
           VehicleDepartureTime = vehicleDepartureTime;
       }
       
       public DateTime DateCreated { get; set; }

       public Guid DocumentRecipientCostCentreId { get; set; }
       public string Note { get; set; }
       public string DriverName { get; set; }
       public string VehicleRegNo { get; set; }
       public Guid RouteId { get; set; }
       public Guid CentreId { get; set; }

       public DateTime? VehicleArrivalTime { get; set; }
       public DateTime? VehicleDepartureTime { get; set; }

       public decimal? VehicleArrivalMileage { get; set; }
       public decimal? VehicleDepartureMileage { get; set; }

       public override string CommandTypeRef
       {
           get { return CommandType.CreateCommodityDelivery.ToString(); }
       }
    }
}
