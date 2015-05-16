using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Domain.Payments
{
    public enum PaymentInstrumentType
    {
        sync = 1,
        async = 2,
        all = 3
    }
}
