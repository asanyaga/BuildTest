using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Data.Util.Caching
{
    public interface ICacheProvider
    {
        void Put(string key, object value);
        object Get(string key);
        void Remove(string key);
    }
}
