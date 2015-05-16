using System;

namespace Distributr.Core.Commands.DocumentCommands.DispatchNotes
{
    public class AddDispatchNoteLineItemCommand : AfterCreateCommand
    {
        public AddDispatchNoteLineItemCommand()
        {
            
        }
        public AddDispatchNoteLineItemCommand(
           Guid commandId,
            Guid documentId,
            Guid commandGeneratedByUserId,
            Guid commandGeneratedByCostCentreId,
            int costCentreApplicationCommandSequenceId,
            Guid commandGeneratedByCostCentreApplicationId,
            int lineItemSequenceNo,
            Guid productId,
            decimal qty,
            decimal lineItemValue,
            decimal lineItemVatValue,
            int lineItemType,
            Guid parentDocId,
            decimal lineItemProductDiscount = 0,
            int discountType = 0//cn: for line items given as discounts

            )
            : base(
            commandId,
            documentId, 
            commandGeneratedByUserId,
            commandGeneratedByCostCentreId,
            costCentreApplicationCommandSequenceId,
            commandGeneratedByCostCentreApplicationId, parentDocId
            )
        {
            LineItemSequenceNo      = lineItemSequenceNo;
            ProductId               = productId;
            Qty                     = qty;
            Value                   = lineItemValue;
            LineItemVatValue        = lineItemVatValue;
            LineItemType            = lineItemType;
            LineItemProductDiscount = lineItemProductDiscount;
            DiscountType            = discountType;
        }

        public int LineItemSequenceNo { get; set; }

        public Guid ProductId { get; set; }
        public decimal Qty { get; set; }
        public decimal Value { get; set; }
        public int LineItemType { get; set; }
        public decimal LineItemProductDiscount { get; set; }
        public int DiscountType { get; set; }
        public decimal LineItemVatValue { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.AddDispatchNoteLineItem.ToString(); }
        }
    }
}
