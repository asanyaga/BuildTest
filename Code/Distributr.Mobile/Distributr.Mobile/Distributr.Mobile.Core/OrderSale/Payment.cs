using System;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Transactional;
using SQLiteNetExtensions.Attributes;

namespace Distributr.Mobile.Core.OrderSale
{
    public enum PaymentStatus { New, Confirmed }

    public class Payment : MasterEntity
    {
        [ForeignKey(typeof(Order))]
        public Guid SaleMasterId { get; set;}
        public PaymentMode PaymentMode { get; set; }
        public decimal Amount { get; set; }
        public string PaymentReference { get; set; }
        public string Description { get; set; }
        public string Bank { get; set; }
        public string BankBranch { get; set; }
        public DateTime DueDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

        public Payment() : base(default(Guid))
        {
        }

        public Payment(Guid saleMasterId): base(Guid.NewGuid())
        {
            SaleMasterId = saleMasterId;
        }
    }
}