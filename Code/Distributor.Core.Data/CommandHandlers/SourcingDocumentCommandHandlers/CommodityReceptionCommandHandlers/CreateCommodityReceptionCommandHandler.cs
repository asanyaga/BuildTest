using System;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityReceptionCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityReceptionCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityReceptionCommandHandlers
{
   public class CreateCommodityReceptionCommandHandler:BaseSourcingCommandHandler,ICreateCommodityReceptionCommandHandler
    {

       ILog _log = LogManager.GetLogger("CreateCommodityReceptionCommandHandler");

       public CreateCommodityReceptionCommandHandler(CokeDataContext context) : base(context)
       {
       }

       public void Execute(CreateCommodityReceptionCommand command)
       {
           _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
           try
           {
               if (DocumentExists(command.DocumentId))
                   return;
               tblSourcingDocument doc = NewDocument(command, DocumentType.CommodityReceptionNote, command.DocumentRecipientCostCentreId);
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
               _log.Error("CreateCommodityDeliveryCommandHandler exception ", ex);
               throw;
           }
       }
    }
}
