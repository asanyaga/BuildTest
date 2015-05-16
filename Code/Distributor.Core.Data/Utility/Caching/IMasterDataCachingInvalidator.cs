using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distributr.Core.Data.Utility.Caching
{
    public interface IMasterDataCachingInvalidator
    {
        void InvalidateMasterDataCaching();
    }
}
