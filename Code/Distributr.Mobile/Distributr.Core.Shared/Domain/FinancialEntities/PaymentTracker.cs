using System;
using Distributr.Core.Domain.Master;
using Distributr.Core.Domain.Transactional;
#if __MOBILE__
using SQLite.Net.Attributes;
#endif

namespace Distributr.Core.Domain.FinancialEntities
{
   public  class PaymentTracker: MasterEntity
    {
        public PaymentTracker() : base(default(Guid)) { }
        public PaymentTracker(Guid id)
            : base(id)
        { }
        public PaymentTracker(Guid id, DateTime dateCreated, DateTime dateLastUpdated, EntityStatus isActive)
            : base(id, dateCreated, dateLastUpdated, isActive)
        { }
        public Guid CostcentreId { get; set; }
    #if __MOBILE__
        [Column("PaymentModeId")]
       
    #endif
        public PaymentMode PaymentMode { get; set; }
        public decimal Balance { get; set; }
        public decimal PendingConfirmBalance { get; set; }
    }
}
