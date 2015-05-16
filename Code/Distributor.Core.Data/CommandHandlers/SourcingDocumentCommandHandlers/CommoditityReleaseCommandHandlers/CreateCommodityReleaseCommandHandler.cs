using System;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityReleaseCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityPurchaseCommands;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityReleaseCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommoditityReleaseCommandHandlers
{
    public class CreateCommodityReleaseCommandHandler :BaseSourcingCommandHandler, ICreateCommodityReleaseCommandHandler
    {

        ILog _log = LogManager.GetLogger("CreateCommodityReleaseCommandHandler");

        public CreateCommodityReleaseCommandHandler(CokeDataContext context)
            : base(context)
        {
        }

        public void Execute(CreateCommodityReleaseCommand command)
        {
            _log.InfoFormat("Execute CreateCommodityReleaseCommandHandler - Command Id {0} ", command.CommandId);
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (DocumentExists(command.DocumentId))
                    return;
                tblSourcingDocument doc = NewDocument(command, DocumentType.CommodityRelease, command.DocumentRecipientCostCentreId);
                doc.Description = command.Description;
                _context.tblSourcingDocument.AddObject(doc);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("CreateCommodityReleaseCommandHandler exception ", ex);
                throw;
            }
            
        }
    }
}
