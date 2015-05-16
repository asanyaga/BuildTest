using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Domain.Notifications
{
   public class Easy247Payment:PgNotification
    {
        public string BillerNumber { get; set; }
        public string OutletPhoneNumber { get; set; }
    }
}
