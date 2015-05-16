using System;
using System.Linq;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Receipts;
using Distributr.Core.Commands.DocumentCommands.Receipts;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Receipts
{
    [Obsolete]
    public class ConfirmReceiptLineItemCommandHandler :BaseCommandHandler, IConfirmReceiptLineItemCommandHandler
    {
        private IReceiptRepository _documentRepository;
        private CokeDataContext _cokeDataContext;
        ILog _log = LogManager.GetLogger("AddReceiptLineItemCommandHandler");

        public ConfirmReceiptLineItemCommandHandler(IReceiptRepository documentRepository, CokeDataContext context)
            :base(context)
        {
            _documentRepository = documentRepository;
        }

        public void Execute(ConfirmReceiptLineItemCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                Receipt receipt = _documentRepository.GetById(command.DocumentId);
               
                bool already_Exist = receipt != null;
                if (!already_Exist)
                    return;

                ReceiptLineItem li = receipt.LineItems.First(n => n.Id == command.LineItemId);

                li.LineItemType = OrderLineItemType.PostConfirmation;
                li.PaymentRefId = command.PaymentRefId;
                li.Description  = command.Description;

                _documentRepository.Save(receipt);
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
               _log.Error("ConfirmReceiptLineItemCommandHandler exception", ex);
                throw ;
            }
        }
    }
}
