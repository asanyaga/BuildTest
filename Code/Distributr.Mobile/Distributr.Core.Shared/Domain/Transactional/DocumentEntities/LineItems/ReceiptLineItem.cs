using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Domain.FinancialEntities;
using Distributr.Core.Domain.Master.BankEntities;

namespace Distributr.Core.Domain.Transactional.DocumentEntities.LineItems
{
   public class ReceiptLineItem : DocumentLineItem
    {
        public ReceiptLineItem(Guid id) : base(id) { }

        public decimal Value { get; set; }
        public Guid PaymentDocLineItemId { get; set; }
        //public AccountType AccountType { get; set; }
        public Bank bank { get; set; }
        public BankBranch bankBranch { get; set; }

        public PaymentMode PaymentType { get; set; }
        public string PaymentRefId { get; set; }
        public string MMoneyPaymentType { get; set; }
        public OrderLineItemType LineItemType { get; set; }//for approval of receipt line items: payments need to be confirmed
        public string NotificationId { get; set; }
    }

}
