using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.Invoices
{
    public class AddInvoiceLineItemCommand : AfterCreateCommand
    {
        public AddInvoiceLineItemCommand()
        {
            
        }
        public AddInvoiceLineItemCommand(
            //Command
            Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,

              int lineItemSequenceNo,
            decimal valueLineItem,
            Guid productId,
            decimal qty,
            decimal lineItemVatValue,
            Guid lineItemId,
            Guid parentDocId,
            string description,
            decimal lineItemProductDiscount = 0,
            int lineItemType = 0,
            int discountType = 0//cn: for line items given as discounts
            )
            : base(commandId, documentId, commandGeneratedByUserId,
                commandGeneratedByCostCentreId,
                costCentreApplicationCommandSequenceId,
                commandGeneratedByCostCentreApplicationId, parentDocId)
        {
            LineItemSequenceNo = lineItemSequenceNo;
            ValueLineItem = valueLineItem;
            ProductId = productId;
            Qty = qty;
            LineItemVatValue = lineItemVatValue;
            LineItemId = lineItemId;
            LineItemProductDiscount = lineItemProductDiscount;
            LineItemType = lineItemType;
            DiscountType = discountType;//cn
            Description = description;
        }

        public int LineItemSequenceNo { get; set; }
        public decimal ValueLineItem { get; set; }
        public Guid ProductId { get; set; }
        public decimal Qty { get; set; }
        public decimal LineItemVatValue { get; set; }
        public decimal LineItemProductDiscount { get; set; }//cn: use this to pin product discount
        public Guid LineItemId { get; set; }
        public int LineItemType { get; set; }//cn: use this to specify the discount line item
        public int DiscountType { get; set; } //cn: for line items given as discounts
        public override string CommandTypeRef
        {
            get { return CommandType.AddInvoiceLineItem.ToString(); }
        }          
    }
}
