using System;
using System.Linq;
using Distributr.Core.CommandHandler.ActivityDocumentHandlers;
using Distributr.Core.Commands.ActivityDocumentCommands;
using Distributr.Core.Data.EF;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.ActivityDocumentHandlers
{
    public class AddActivityProduceLineItemCommandHandler : IAddActivityProduceLineItemCommandHandler
    {

        ILog _log = LogManager.GetLogger("AddActivityProduceLineItemCommandHandler");
        CokeDataContext _context;
        public AddActivityProduceLineItemCommandHandler(CokeDataContext context)
        {
            _context = context;
        }

        public void Execute(AddActivityProduceLineItemCommand command)
        {
            _log.InfoFormat("Execute AddActivityProduceLineItemCommandHandler - Command Id {0} ", command.CommandId);
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!_context.tblActivityDocument.Any(s => s.Id == command.DocumentId))
                {
                    _log.InfoFormat("Cannot add line item. Document does not exist  Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                    return;
                }

                if (_context.tblActivityProduceLineItem.Any(s => s.Id == command.LineItemId))
                {
                    _log.InfoFormat("Cannot add line item {0}. Line item already exists", command.CommandId);
                    return;
                }
                tblActivityProduceLineItem doc = new tblActivityProduceLineItem();
                doc.Id = command.LineItemId;
                doc.ActivityId = command.DocumentId;
                doc.CommodityId = command.CommodityId;
                doc.GradeId = command.GradeId;
                doc.Weight = command.Weight;
                doc.ServiceProviderId = command.ServiceProviderId;
                doc.Description = command.Description;
                doc.IM_DateCreated = DateTime.Now;
                doc.IM_DateLastUpdated = DateTime.Now;
                doc.IM_Status = 1;
                _context.tblActivityProduceLineItem.AddObject(doc);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("AddActivityProduceLineItemCommandHandler exception ", ex);
                throw;
            }

        }
    }
}