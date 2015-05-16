using System;
using Distributr.Core.CommandHandler.DocumentCommandHandlers.Invoices;
using Distributr.Core.Commands.DocumentCommands.Invoices;
using Distributr.Core.Data.EF;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;
using Distributr.Core.Repository.Master.ProductRepositories;
using Distributr.Core.Repository.Transactional.DocumentRepositories.IInvoiceRepositories;
using log4net;

namespace Distributr.Core.Data.CommandHandlers.DocumentCommandHandlers.Invoices
{
    public class AddInvoiceLineItemCommandHandler : BaseCommandHandler, IAddInvoiceLineItemCommandHandler
    {
        private CokeDataContext _cokeDataContext;
        IInvoiceRepository _documentRepository;
        IProductRepository _productRepository;
        ILog _log = LogManager.GetLogger("AddInvoiceLineItemCommandHandler");

        public AddInvoiceLineItemCommandHandler(IInvoiceRepository documentRepository, IProductRepository productRepository, CokeDataContext cokeDataContext) 
            : base(cokeDataContext)
        {
            _cokeDataContext = cokeDataContext;
            _documentRepository = documentRepository;
            _productRepository = productRepository;
        }


        public void Execute(AddInvoiceLineItemCommand command)
        {
            _log.InfoFormat("Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
            try
            {
                if (!DocumentExists(command.DocumentId))
                {
                    _log.InfoFormat("Cannot add line item. Document does not exist  Execute {1} - Command Id {0} ", command.CommandId, command.GetType());
                    return;
                }
               
                if (DocumentLineItemExists(command.CommandId))
                {
                    _log.InfoFormat("Cannot add line item {0}. Line item already exists", command.CommandId);
                    return;
                }

                tblDocument doc = ExistingDocument(command.DocumentId);
                tblLineItems lineItem = NewLineItem(command.CommandId, command.DocumentId, command.ProductId,
                                                    command.Description, command.Qty, command.LineItemSequenceNo);
                lineItem.Value = command.ValueLineItem;
                lineItem.Vat = command.LineItemVatValue;
                lineItem.ProductDiscount = command.LineItemProductDiscount;
                lineItem.OrderLineItemType = command.LineItemType;
                lineItem.DiscountLineItemTypeId =  command.DiscountType;
                doc.tblLineItems.Add(lineItem);
                _cokeDataContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Error Execute {1} - Command Id {0} ", command.CommandId, command.GetType().ToString());
                _log.Error("AddInvoiceLineItemCommandHandler exception", ex);
                throw ;
            }
        }
    }
}
