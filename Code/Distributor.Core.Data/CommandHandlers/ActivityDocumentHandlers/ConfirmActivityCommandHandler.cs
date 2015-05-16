using System;
using System.Linq;
using Distributr.Core.CommandHandler.ActivityDocumentHandlers;
using Distributr.Core.Commands.ActivityDocumentCommands;
using Distributr.Core.Data.EF;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.ActivityDocumentHandlers
{
    public class ConfirmActivityCommandHandler : IConfirmActivityCommandHandler
    {

        ILog _log = LogManager.GetLogger("ConfirmActivityCommandHandler");
        CokeDataContext _context;
        public ConfirmActivityCommandHandler(CokeDataContext context)
        {
            _context = context;
        }

        public void Execute(ConfirmActivityCommand command)
        {
            _log.InfoFormat("Execute ConfirmActivityCommandHandler - Command Id {0} ", command.CommandId);
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
              
                tblActivityDocument doc = _context.tblActivityDocument.FirstOrDefault(s => s.Id == command.DocumentId);
                if (doc==null)
                    return;
                doc.IM_Status = 1;
               
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("ConfirmActivityCommandHandler exception ", ex);
                throw;
            }

        }
    }
}