using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Transactional.DocumentEntities.LineItems
{
    public class DispatchNoteLineItem : OrderLineItem
    {
        public DispatchNoteLineItem(Guid id) : base(id)
        {
        }

        public override decimal LineItemTotal
        {
            get
            {
                return Qty * Value;
            }
        }
    }
}
