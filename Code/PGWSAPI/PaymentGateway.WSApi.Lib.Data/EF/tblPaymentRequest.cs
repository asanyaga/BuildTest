//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PaymentGateway.WSApi.Lib.Data.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblPaymentRequest
    {
        public System.Guid Id { get; set; }
        public System.Guid DistributorCostCenterId { get; set; }
        public string TransactionRefId { get; set; }
        public string SDPTransactionRefId { get; set; }
        public string SDPReferenceId { get; set; }
        public string SubscriberName { get; set; }
        public string ReceiptNumber { get; set; }
        public Nullable<int> ClientRequestResponseTypeId { get; set; }
        public string ApplicationId { get; set; }
        public string SubscriberId { get; set; }
        public string PaymentInstrumentName { get; set; }
        public string AccountId { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<double> TotalAmount { get; set; }
        public Nullable<double> BalanceDue { get; set; }
        public Nullable<double> MerchantBalance { get; set; }
        public string Currency { get; set; }
        public string InvoiceNumber { get; set; }
        public string OrderNumber { get; set; }
        public string StatusCode { get; set; }
        public string StatusDetail { get; set; }
        public string Extra { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<System.DateTime> Time { get; set; }
        public Nullable<System.DateTime> TimeStamp { get; set; }
        public System.DateTime DateCreated { get; set; }
    }
}