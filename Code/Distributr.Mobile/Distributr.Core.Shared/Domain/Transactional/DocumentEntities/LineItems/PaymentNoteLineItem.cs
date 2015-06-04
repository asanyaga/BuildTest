using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Transactional.DocumentEntities.LineItems
{
    public class PaymentNoteLineItem : DocumentLineItem
    {
        public PaymentNoteLineItem(Guid id)
            : base(id)
        {
        }
        
        public decimal Amount { get; set; }
        public PaymentMode PaymentMode { get; set; }
        
    }
}
