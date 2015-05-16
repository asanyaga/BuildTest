using System;

namespace Distributr.Core.Commands.DocumentCommands.Orders
{
     [Obsolete]
    public class AddOrderLineItemCommand : AfterCreateCommand
    {
        public AddOrderLineItemCommand()
        {
            
        }
        public AddOrderLineItemCommand(
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
            decimal productDiscount,
            string description,
            int lineItemType = 0,//cn
            int discountType = 0//cn: for line items given as discounts
            )
            : base(commandId, documentId, commandGeneratedByUserId,
                    commandGeneratedByCostCentreId,
                    costCentreApplicationCommandSequenceId,
                    commandGeneratedByCostCentreApplicationId,documentId)
        {
            LineItemSequenceNo = lineItemSequenceNo;
            ValueLineItem = valueLineItem;
            ProductId = productId;
            Qty = qty;
            ProductDiscount = productDiscount;
            LineItemVatValue = lineItemVatValue;
            Description = description;
            LineItemType = lineItemType;//cn
            DiscountType = discountType;//cn
        }
        

        public int LineItemSequenceNo { get; set; }
        public decimal ValueLineItem { get; set; }
        public decimal ProductDiscount { get; set; }
        public Guid ProductId { get; set; }
        public decimal Qty { get; set; }
        public decimal LineItemVatValue { get; set; }
        public int LineItemType { get; set; } //cn
        public int DiscountType { get; set; } //cn: for line items given as discounts
        public override string CommandTypeRef
        {
            get { return CommandType.AddOrderLineItem.ToString(); }
        }
    }

    public class AddExternalDocRefCommand : AfterCreateCommand
    {
        public override string CommandTypeRef
        {
            get { return CommandType.AddExternalDocRef.ToString(); }
        }
        public string ExternalDocRef { get; set; }
    }

    public class AddMainOrderLineItemCommand : AfterCreateCommand
    {
        public AddMainOrderLineItemCommand()
        {

        }
        public AddMainOrderLineItemCommand(
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
            decimal productDiscount,
            string description,
            int lineItemType = 0,//cn
            int discountType = 0//cn: for line items given as discounts
            )
            : base(commandId, documentId, commandGeneratedByUserId,
                    commandGeneratedByCostCentreId,
                    costCentreApplicationCommandSequenceId,
                    commandGeneratedByCostCentreApplicationId, documentId)
        {
            LineItemSequenceNo = lineItemSequenceNo;
            ValueLineItem = valueLineItem;
            ProductId = productId;
            Qty = qty;
            ProductDiscount = productDiscount;
            LineItemVatValue = lineItemVatValue;
            Description = description;
            LineItemType = lineItemType;//cn
            DiscountType = discountType;//cn
            
        }


        public int LineItemSequenceNo { get; set; }
        public decimal ValueLineItem { get; set; }
        public decimal ProductDiscount { get; set; }
        public Guid ProductId { get; set; }
        public decimal Qty { get; set; }
      
        public decimal LineItemVatValue { get; set; }
        public int LineItemType { get; set; } //cn
        public int DiscountType { get; set; } //cn: for line items given as discounts
        public override string CommandTypeRef
        {
            get { return CommandType.AddMainOrderLineItem.ToString(); }
        }
    }
}
