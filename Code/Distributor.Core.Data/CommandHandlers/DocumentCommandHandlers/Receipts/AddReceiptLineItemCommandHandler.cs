using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Receipts;
using Distributr.Core.Commands.DocumentCommands.Receipts;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Transactional.DocumentRepositories.ReceiptInventories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Receipts
{
    public class AddReceiptLineItemCommandHandler : IAddReceiptLineItemCommandHandler
    {
        private IReceiptRepository _documentRepository;
        ILog _log = LogManager.GetLogger("AddReceiptLineItemCommandHandler");

        public AddReceiptLineItemCommandHandler(IReceiptRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public void Execute(AddReceiptLineItemCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                bool already_Exist = _documentRepository.GetById(command.DocumentId) != null;
                if (!already_Exist)
                    return;
                Receipt r = _documentRepository.GetById(command.DocumentId) as Receipt;
                ReceiptLineItem li = new ReceiptLineItem(command.LineItemId);

                li.LineItemSequenceNo   = command.LineItemSequenceNo;
                li.Value                = command.Value;
                li.Description          = command.Description;
                li.PaymentType          = (PaymentMode) command.PaymentTypeId;
                li.PaymentRefId         = command.PaymentTypeReference;
                li.LineItemType         = (OrderLineItemType)command.LineItemType;
                li.MMoneyPaymentType    = command.MMoneyPaymentType;
                li.PaymentDocLineItemId = command.PaymentDocLineItemId;
                li.NotificationId       = command.NotificationId;

                r.Add(li);

                _documentRepository.Save(r);
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("AddReceiptLineItemCommandHandler exception", ex);
                throw;
            }
        }
    }
}
