using System;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityWarehouseStorageCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityWarehouseStorageCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityWarehouseStorageCommandHandlers
{
    public class CreateCommodityWarehouseStorageCommandHandler :BaseSourcingCommandHandler, ICreateCommodityWarehouseStorageCommandHandler
    {

        ILog _log = LogManager.GetLogger("CreateCommodityWarehouseStorageCommandHandler");
        public CreateCommodityWarehouseStorageCommandHandler(CokeDataContext context)
            : base(context)
        {
        }

        public void Execute(CreateCommodityWarehouseStorageCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (DocumentExists(command.DocumentId))
                    return;
                tblSourcingDocument doc = NewDocument(command, DocumentType.CommodityWarehouseStorage, command.DocumentRecipientCostCentreId);
                doc.DriverName = command.DriverName;
                doc.VehicleRegNo = command.VehicleRegNo;
                doc.RouteId = command.RouteId;
                doc.CentreId = command.CentreId;
                doc.DocumentIssuerUserId = command.CommandGeneratedByUserId;
                doc.CommodityOwnerId = command.CommodityOwnerId;
                doc.VehicleArrivalMileage = command.VehicleArrivalMileage;
                doc.VehicleArrivalTime = command.VehicleArrivalTime;
                doc.VehicleDepartureMileage = command.VehicleDepartureMileage;
                doc.VehicleDepartureTime = command.VehicleDepartureTime;
                _context.tblSourcingDocument.AddObject(doc);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("CreateCommodityWarehouseStorageCommandHandler exception ", ex);
                throw;
            }
        }
    }
}
