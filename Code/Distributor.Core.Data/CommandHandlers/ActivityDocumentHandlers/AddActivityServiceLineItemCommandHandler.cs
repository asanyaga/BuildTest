using System;
using System.Linq;
using Distributr.Core.CommandHandler.ActivityDocumentHandlers;
using Distributr.Core.Commands.ActivityDocumentCommands;
using Distributr.Core.Data.EF;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.ActivityDocumentHandlers
{
    public class AddActivityServiceLineItemCommandHandler : IAddActivityServiceLineItemCommandHandler
    {

        ILog _log = LogManager.GetLogger("AddActivityServiceLineItemCommandHandler");
        CokeDataContext _context;
        public AddActivityServiceLineItemCommandHandler(CokeDataContext context)
        {
            _context = context;
        }

        public void Execute(AddActivityServiceLineItemCommand command)
        {
            _log.InfoFormat("Execute AddActivityServiceLineItemCommandHandler - Command Id {0} ", command.CommandId);
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!_context.tblActivityDocument.Any(s => s.Id == command.DocumentId))
                {
                    _log.InfoFormat("Cannot add line item. Document does not exist  Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                    return;
                }

                if (_context.tblActivityServiceLineItem.Any(s => s.Id == command.LineItemId))
                {
                    _log.InfoFormat("Cannot add line item {0}. Line item already exists", command.CommandId);
                    return;
                }
                tblActivityServiceLineItem doc = new tblActivityServiceLineItem();
                doc.Id = command.LineItemId;
                doc.ActivityId = command.DocumentId;
                doc.ServiceId = command.ServiceId;
                doc.ServiceProviderId = command.ServiceProviderId;
                doc.ShiftId = command.ShiftId;
               
                doc.Description = command.Description;
                doc.IM_DateCreated = DateTime.Now;
                doc.IM_DateLastUpdated = DateTime.Now;
                doc.IM_Status = 1;
                _context.tblActivityServiceLineItem.AddObject(doc);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("AddActivityServiceLineItemCommandHandler exception ", ex);
                throw;
            }

        }
    }
}