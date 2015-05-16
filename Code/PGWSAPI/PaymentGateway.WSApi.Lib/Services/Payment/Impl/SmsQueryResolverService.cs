using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.WSApi.Lib.Services.Payment.Impl
{
    public class SmsQueryResolverService : ISmsQueryResolverService
    {
        public string SmsQuery(SmsQuery query)
        {
            return "TEST QUERY";
        }
    }
}
