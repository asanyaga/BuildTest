using System;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityStorageCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityStorageCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.SourcingDocumentEnitities;
using Distributr.Core.Repository.Master.CostCentreRepositories;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.SourcingDocumentRepositories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommoditityStorageCommandHandlers
{
    public class CreateCommodityStorageCommandHandler :BaseSourcingCommandHandler, ICreateCommodityStorageCommandHandler
    {
        ILog _log = LogManager.GetLogger("CreateCommodityStorageCommandHandler");


        public CreateCommodityStorageCommandHandler(CokeDataContext context) : base(context)
        {
        }

        public void Execute(CreateCommodityStorageCommand command)
        {
            _log.InfoFormat("Execute  - Command Id {0} ", command.CommandId);
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (DocumentExists(command.DocumentId))
                    return;
                tblSourcingDocument doc = NewDocument(command, DocumentType.CommodityStorageNote, command.DocumentRecipientCostCentreId);
                _context.tblSourcingDocument.AddObject(doc);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("CreateCommodityStorageCommandHandler exception ", ex);
                throw;
            }
        }
    }
}
