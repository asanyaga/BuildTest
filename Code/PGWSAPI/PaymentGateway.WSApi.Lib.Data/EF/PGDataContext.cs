using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Data.EF
{
    public class PGDataContext:paymentgatewayEntities
    {
        public PGDataContext(string connectionString)
            : base(connectionString)
        {

        }
    }
}
