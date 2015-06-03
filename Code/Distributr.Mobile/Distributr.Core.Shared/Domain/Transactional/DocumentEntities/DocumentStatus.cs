using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Domain.Transactional.DocumentEntities
{
    public enum DocumentStatus
    {
        New       = 0,
        Confirmed = 1,
        Submitted = 2,
        Cancelled = 3,
        Approved  = 4,
        Dispatched=6,
        Rejected  = 5,

        //Orders
        OrderPendingDispatch     = 50,
        OrderDispatchedToPhone   = 51,
        OrderBackOrder           = 52,
        Closed                   = 99,
        OrderLossSale=53,
        Outstanding =54,
        UnconfirmedReceiptPayment=55,
        FullyPaidOrders=56
    }
    public enum OrderStatus
    {
        None=0,
        New = 1,
        Inprogress = 2,
        Complete = 3,
        Cancelled=4,
       
    }
}
