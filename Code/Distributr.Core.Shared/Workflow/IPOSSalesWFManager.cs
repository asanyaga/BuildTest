using System;
using System.Collections.Generic;
using Distributr.Core.ClientApp;
using Distributr.Core.Domain.Transactional;
using Distributr.Core.Domain.Transactional.DocumentEntities;
using Distributr.Core.Domain.Transactional.DocumentEntities.LineItems;

namespace Distributr.Core.Workflow
{
    public class PaymentInfo
    {
        public Guid Id { get; set; }
        public PaymentMode PaymentModeUsed {get;set;}
        public decimal Amount {get;set;}
        public string PaymentRefId {get;set;}
        public string MMoneyPaymentType { get; set; }
        public string PaymentTypeDisplayer { get; set; }
        public string Description { get; set; }
        public bool IsNew { get; set; }
        public bool IsConfirmed { get; set; }
        public bool CanEdit { get { return IsNew && (PaymentModeUsed != PaymentMode.MMoney && PaymentModeUsed != PaymentMode.Credit); } }
        public bool CanRemove { get { return IsNew && (PaymentModeUsed != PaymentMode.Credit); } }
        public bool CanConfirm { get { return !IsConfirmed; } }
        public Guid InvoiceId { get; set; }
        public string InvoiceDocRef { get; set; }
        public Guid ReceiptId { get; set; }
        public string ReceiptDocRef { get; set; }
        public string NotificationId { get; set; }
        public bool IsProcessed { get; set; }
        public decimal ConfirmedAmount{get; set; }

        public string Bank { get; set; }
        public string BankBranch { get; set; }
        public DateTime? DueDate { get; set; }
    }
   
    [Obsolete("AJM found no references in code to this contract")]
    public interface IPOSSalesWFManager 
    {
        void SubmitChanges(Order order);
        Receipt SubmitPaymentChanges(Order order, List<PaymentInfo> paymentInfo, Guid invoiceId, string invoiceRef, BasicConfig config);
        void SubmitSecondarymMoneyPayment(Order order, Receipt paymentDoc, List<PaymentInfo> paymentInfo, Guid invoiceId, string invoiceRef, BasicConfig config);
        void ConfirmOrder(Order order, BasicConfig config);
        void CloseOrder(Order document, BasicConfig config);
        void RejectOrder(Order order, BasicConfig config);
        void ConfirmReceiptLineItem(ReceiptLineItem item, Receipt receipt, BasicConfig config);
    }
}
