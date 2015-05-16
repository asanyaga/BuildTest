using System;
using System.Linq;
using Distributr.Core.CommandHandler.ActivityDocumentHandlers;
using Distributr.Core.Commands.ActivityDocumentCommands;
using Distributr.Core.Data.EF;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.ActivityDocumentHandlers
{
    public class AddActivityInputLineItemCommandHandler : IAddActivityInputLineItemCommandHandler
    {

        ILog _log = LogManager.GetLogger("AddActivityInputLineItemCommandHandler");
        CokeDataContext _context;
        public AddActivityInputLineItemCommandHandler(CokeDataContext context)
        {
            _context = context;
        }

        public void Execute(AddActivityInputLineItemCommand command)
        {
            _log.InfoFormat("Execute AddActivityInputLineItemCommandHandler - Command Id {0} ", command.CommandId);
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!_context.tblActivityDocument.Any(s => s.Id == command.DocumentId))
                {
                    _log.InfoFormat("Cannot add line item. Document does not exist  Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                    return;
                }

                if (_context.tblActivityInputLineItem.Any(s => s.Id == command.LineItemId))
                {
                    _log.InfoFormat("Cannot add line item {0}. Line item already exists", command.CommandId);
                    return;
                }
                tblActivityInputLineItem doc = new tblActivityInputLineItem();
                doc.Id = command.LineItemId;
                doc.ActivityId = command.DocumentId;
                doc.ProductId = command.ProductId;
                doc.Quantity = command.Quantity;
                doc.MF_Date = command.ManufacturedDate;
                doc.EP_Date = command.ExpiryDate;
                doc.Description = command.Description;
                doc.Description = command.Description;
                doc.IM_DateCreated = DateTime.Now;
                doc.IM_DateLastUpdated = DateTime.Now;
                doc.IM_Status = 1;
                _context.tblActivityInputLineItem.AddObject(doc);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("AddActivityInputLineItemCommandHandler exception ", ex);
                throw;
            }

        }
    }
}