using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Transactional.DocumentEntities.LineItems
{
    public class DiscountLineItem : ProductLineItem
    {
        public DiscountLineItem(Guid id)
            : base(id)
        {
        }

        public enum DiscountLineItemType
        {
            Product = 1,
            Value = 2,
            Deleted = 3
        }

        public DiscountLineItemType Discount_LineItemType { get; set; }
        public decimal LineItemTotal
        {
            get
            {
                if (Discount_LineItemType == DiscountLineItemType.Product) return (Qty * Value);
                else if (Discount_LineItemType == DiscountLineItemType.Value) return Value;
                return (Qty*Value);
            }
        }
    }
}
