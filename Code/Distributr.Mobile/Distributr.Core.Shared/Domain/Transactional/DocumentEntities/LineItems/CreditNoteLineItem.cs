using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Transactional.DocumentEntities.LineItems
{
    public class CreditNoteLineItem : ProductLineItem
    {
        public CreditNoteLineItem(Guid id) : base(id) { }


        public decimal LineItemVatValue { get; set; }

        public OrderLineItemType LineItemType { get; set; }

        public decimal LineItemVatTotal
        {
            get { return Qty * LineItemVatValue; }
        }

        public decimal LineItemTotal
        {
            get
            {
                return LineItemVatTotal + (Qty * Value);
            }
        }
    }
}
