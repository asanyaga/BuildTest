using System;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityPurchaseCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityPurchaseCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Master.CostCentreEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Repository.Master.CommodityOwnerRepository;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommoditityPurchaseCommandHandlers
{
    public class CreateCommodityPurchaseCommandHandler :BaseSourcingCommandHandler, ICreateCommodityPurchaseCommandHandler
    {

        ILog _log = LogManager.GetLogger("CreateCommodityPurchaseCommandHandler");

        public CreateCommodityPurchaseCommandHandler(CokeDataContext context) : base(context)
        {
        }

        public void Execute(CreateCommodityPurchaseCommand command)
        {
            _log.InfoFormat("Execute CreateCommodityPurchaseCommandHandler - Command Id {0} ", command.CommandId);
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (DocumentExists(command.DocumentId))
                    return;
                tblSourcingDocument doc = NewDocument(command, DocumentType.CommodityPurchaseNote, command.DocumentRecipientCostCentreId);
                doc.CommodityOwnerId = command.CommodityOwnerId;
                doc.CommodityProducerId = command.CommodityProducerId;
                doc.DocumentOnBehalfOfCostCentreId = command.CommoditySupplierCostCentreId;
                doc.DeliveredBy = command.DeliveredBy;
                doc.CentreId = command.CentreId;
                doc.RouteId = command.RouteId;
                doc.Latitude = command.Latitude;
                doc.Longitude = command.Longitude;

                _context.tblSourcingDocument.AddObject(doc);
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
