using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Domain.Notifications
{
    public class SmscNotification : PgNotification
    {
        public string TillNumber { get; set; }
        public string OutletPhoneNumber { get; set; }
    }
}
