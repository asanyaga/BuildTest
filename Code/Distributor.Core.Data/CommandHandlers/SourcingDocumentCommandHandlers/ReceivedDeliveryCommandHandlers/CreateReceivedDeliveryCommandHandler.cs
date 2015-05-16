using System;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.ReceivedDeliveryCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityDeliveryCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.ReceivedDeliveryCommandHandlers
{
    public class CreateReceivedDeliveryCommandHandler : BaseSourcingCommandHandler, ICreateReceivedDeliveryCommandHandler
    {

        ILog _log = LogManager.GetLogger("CreateReceivedDeliveryCommandHandler");
        public CreateReceivedDeliveryCommandHandler(CokeDataContext context)
            : base(context)
        {
        }

        public void Execute(CreateReceivedDeliveryCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (DocumentExists(command.DocumentId))
                    return;
                tblSourcingDocument doc = NewDocument(
                    command, DocumentType.ReceivedDelivery, command.DocumentRecipientCostCentreId);
                doc.RouteId = command.RouteId;
                doc.CentreId = command.CentreId;
                doc.DocumentRecipientCostCentreId = command.DocumentRecipientCostCentreId;
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
                _log.Error("CreateReceivedDeliveryCommandHandler exception ", ex);
                throw;
            }
        }
    }
}
