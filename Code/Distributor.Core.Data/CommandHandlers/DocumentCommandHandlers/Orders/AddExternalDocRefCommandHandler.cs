using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.CommandHandler.DocumentCommandHandlers;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Orders
{
  public  class AddExternalDocRefCommandHandler: BaseCommandHandler, IAddExternalDocRefCommandHandler
    {
        private CokeDataContext _cokeDataContext;
        ILog _log = LogManager.GetLogger("AddExternalDocRefCommandHandler");

        public AddExternalDocRefCommandHandler(CokeDataContext cokeDataContext)
            : base(cokeDataContext)
        {
            _cokeDataContext = cokeDataContext;
        }

        public void Execute(AddExternalDocRefCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                    return;

                tblDocument doc = _cokeDataContext.tblDocument.FirstOrDefault(s => s.Id == command.DocumentId);
                doc.ExtDocumentReference = command.ExternalDocRef;
                _cokeDataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("AddExternalDocRefCommandHandler exception", ex);
                throw;
            }
        }

    }
}
