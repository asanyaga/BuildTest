using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Transactional
{
    public class InvoicePaymentInfo
    {
        public Guid InvoiceId { get; set; }
        public Guid OrderId { get; set; }
        public decimal InvoiceAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal AmountDue { get; set; }
        public decimal CreditNoteAmount { get; set; }
        public DateTime InvoiceDate { get; set; }
    }
}
