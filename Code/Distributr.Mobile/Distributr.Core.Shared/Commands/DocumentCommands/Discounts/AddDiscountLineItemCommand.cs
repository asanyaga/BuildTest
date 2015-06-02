using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Commands.DocumentCommands.Discounts
{
    public class AddDiscountLineItemCommand : AfterCreateCommand
    {
        public AddDiscountLineItemCommand()
        {
            
        }
        public AddDiscountLineItemCommand(Guid commandId, Guid documentId, 
            Guid commandGeneratedByUserId, Guid commandGeneratedByCostCentreId, 
            int costCentreApplicationCommandSequenceId, 
            Guid commandGeneratedByCostCentreApplicationId, 
            double? longitude, double? latitude, 
            int lineItemSequenceNo, decimal valueLineItem, 
            Guid productId, decimal qty,
            Guid lineItemId, int discountLineItemType, int discountType, Guid parentDocId) 
            : base(commandId, documentId, commandGeneratedByUserId, commandGeneratedByCostCentreId, 
            costCentreApplicationCommandSequenceId, commandGeneratedByCostCentreApplicationId, parentDocId, longitude, latitude)
        {
            LineItemSequenceNo = lineItemSequenceNo;
            ValueLineItem = valueLineItem;
            ProductId = productId;
            Qty = qty;
            LineItemId = lineItemId;
            DiscountLineItemType = discountLineItemType;
            DiscountType = discountType;
        }

        public int LineItemSequenceNo { get; set; }
        public decimal ValueLineItem { get; set; }
        public Guid ProductId { get; set; }
        public decimal Qty { get; set; }
        public Guid LineItemId { get; set; }
        public int DiscountLineItemType { get; set; }
        public int DiscountType { get; set; }
        public override string CommandTypeRef
        {
            get { return CommandType.AddDiscountLineItem.ToString(); }
        }
    }
}
