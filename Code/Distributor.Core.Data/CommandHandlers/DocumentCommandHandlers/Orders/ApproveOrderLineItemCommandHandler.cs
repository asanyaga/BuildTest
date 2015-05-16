using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Orders;
using Distributr.Core.Commands.DocumentCommands.Orders;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Orders
{
    public class ApproveOrderLineItemCommandHandler :BaseCommandHandler, IApproveOrderLineItemCommandHandler
    {
        ILog _log = LogManager.GetLogger("ApproveOrderLineItemCommandHandler");
        private CokeDataContext _cokeDataContext;
        public ApproveOrderLineItemCommandHandler(CokeDataContext context) : base(context)
        {
            _cokeDataContext = context;
        }

        public void Execute(ApproveOrderLineItemCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                {
                    _log.InfoFormat("Cannot approve line item. Document does not exist  Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                    return;
                }
                if (!DocumentLineItemExists(command.LineItemId))
                {
                    _log.InfoFormat("Cannot Update line item {0}. Line item already does not exists", command.CommandId);
                    return;
                }

                tblDocument doc = ExistingDocument(command.DocumentId);
                tblLineItems lineItem = doc.tblLineItems.FirstOrDefault(s => s.id == command.LineItemId);
               
                lineItem.LostSaleQuantity = command.LossSaleQuantity;
                lineItem.BackOrderQuantity = lineItem.Quantity - (command.LossSaleQuantity+command.ApprovedQuantity);
                lineItem.ApprovedQuantity = command.ApprovedQuantity;
                lineItem.LineItemStatusId = (int)MainOrderLineItemStatus.Approved;
                _cokeDataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("AddOrderLineItemCommandHandler exception", ex);
                throw;
            }
        }
    }
}
