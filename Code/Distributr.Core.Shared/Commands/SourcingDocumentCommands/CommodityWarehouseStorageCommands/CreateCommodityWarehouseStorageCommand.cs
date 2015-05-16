using System;
using Distributr.Core.Commands.DocumentCommands;

namespace Distributr.Core.Commands.SourcingDocumentCommands.CommodityWarehouseStorageCommands
{
    public class CreateCommodityWarehouseStorageCommand : CreateCommand
    {
        public CreateCommodityWarehouseStorageCommand()
        {
        }

       
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }

        public Guid DocumentRecipientCostCentreId { get; set; }
        public string Note { get; set; }
        public string DriverName { get; set; }
        public string VehicleRegNo { get; set; }
        public Guid RouteId { get; set; }
        public Guid CentreId { get; set; }
        public Guid CommodityOwnerId { get; set; }

        public DateTime? VehicleArrivalTime { get; set; }
        public DateTime? VehicleDepartureTime { get; set; }

        public decimal? VehicleArrivalMileage { get; set; }
        public decimal? VehicleDepartureMileage { get; set; }

        public override string CommandTypeRef
        {
            get { return CommandType.CreateCommodityWarehouseStorage.ToString(); }
        }
    }
}