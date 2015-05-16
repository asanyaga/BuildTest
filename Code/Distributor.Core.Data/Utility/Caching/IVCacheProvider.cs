using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Data.Utility.Caching
{
   public  interface IVCacheProvider
    {
       void Put<T>(string key);
       object Get<T>(string key);
       bool Invalidate<T>(string key);
    }
}
