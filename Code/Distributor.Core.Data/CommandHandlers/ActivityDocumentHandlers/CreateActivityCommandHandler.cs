using System;
using System.Linq;
using Distributr.Core.CommandHandler.ActivityDocumentHandlers;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityPurchaseCommandHandler;
using Distributr.Core.Commands.ActivityDocumentCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityPurchaseCommands;
using Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.ActivityDocumentHandlers
{
    public class CreateActivityCommandHandler : ICreateActivityCommandHandler
    {

        ILog _log = LogManager.GetLogger("CreateActivityCommandHandler");
         CokeDataContext _context;
        public CreateActivityCommandHandler( CokeDataContext context)

        {
            _context = context;
        }

        public void Execute(CreateActivityNoteCommand command)
        {
            _log.InfoFormat("Execute CreateActivityCommandHandler - Command Id {0} ", command.CommandId);
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (_context.tblActivityDocument.Any(s=>s.Id==command.DocumentId))
                    return;
                tblActivityDocument doc = new tblActivityDocument();
                doc.Id = command.DocumentId;
                doc.ActivityDate = command.ActivityDate;
                doc.ActivityReference = command.DocumentReference;
                doc.ActivityTypeId = command.ActivityTypeId;
                doc.CentreId = command.CentreId;
                doc.RouteId = command.RouteId;
                doc.ClerkId = command.DocumentIssuerCostCentreId;
                doc.CommoditySupplierId = command.CommoditySupplierId;
                doc.Description = command.Description;
                doc.IM_DateCreated = DateTime.Now;
                doc.IM_DateLastUpdated = DateTime.Now;
                doc.IM_Status =0;
                doc.SeasonId = command.SeasonId;
                doc.hubId = command.HubId;
                doc.ActivityReference = command.DocumentReference;
                doc.CommodityProducerId = command.CommodityProducerId;
                
                
                _context.tblActivityDocument.AddObject(doc);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("CreateCommodityPurchaseCommandHandler exception ", ex);
                throw;
            }
            
        }
    }
}
