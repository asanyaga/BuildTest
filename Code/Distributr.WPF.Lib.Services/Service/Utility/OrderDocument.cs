using System;
using System.Collections.Generic;
using System.Linq;
using Distributr.Core.Domain.Transactional.DocumentEntities;

namespace Distributr.WPF.Lib.Services.Service.Utility
{
    public class OrderDocument
    {
        public OrderDocument(Order order, Invoice invoices, List<Receipt> receipts)
        {
            _Order = order;
            _Invoices = invoices;
            Receipts = receipts;
            CalcAmountPaid();
        }

        public Order _Order { get; set; }
        //public List<Invoice> _Invoices { get; set; }
        public Invoice _Invoices { get; set; }
        public List<Receipt> Receipts { get; set; }
        public Decimal AmountPaid { get; set; }
        public Decimal AmountDue { get; set; }

        void CalcAmountPaid()
        {
            decimal inAmt = _Invoices != null ? _Invoices.TotalGross : 0;

            AmountPaid = Receipts.Sum(n => n.Total);
            AmountDue = inAmt - AmountPaid;
        }
    }
}