using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.CommandHandler.SourcingDocumentCommandHandlers.CommodityTranferCommandHandler;
using Distributr.Core.Commands.SourcingDocumentCommands.CommodityTransferCommands;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.SourcingDocumentCommandHandlers.CommodityTransferCommandHandlers
{
    public class CreateCommodityTransferCommandHandler:BaseSourcingCommandHandler, ICreateCommodityTransferCommandHandler
    {
        ILog _log = LogManager.GetLogger("CreateCommodityTransferCommandHandler");


        public CreateCommodityTransferCommandHandler(CokeDataContext context)
            : base(context)
        {
        }

        public void Execute(CreateCommodityTransferCommand command)
        {
            _log.InfoFormat("Execute  - Command Id {0} ", command.CommandId);
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (DocumentExists(command.DocumentId))
                    return;
                tblSourcingDocument doc = NewDocument(command, DocumentType.CommodityTransferNote, command.DocumentRecipientCostCentreId);
                doc.DocumentOnBehalfOfCostCentreId = command.WareHouseToStore;
                doc.DocumentTypeId2 = command.DocumentTypeId2;
                _context.tblSourcingDocument.AddObject(doc);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("CreateCommodityTransferCommandHandler exception ", ex);
                throw;
            }
        }
    }
}
