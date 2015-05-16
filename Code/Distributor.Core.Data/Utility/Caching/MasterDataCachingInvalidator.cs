using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Distributr.Core.Utility.Caching;

namespace Distributr.Core.Data.Utility.Caching
{

    public class MasterDataCachingInvalidator : IMasterDataCachingInvalidator
    {
        ICacheProvider _cacheProvider;

        public MasterDataCachingInvalidator(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public void InvalidateMasterDataCaching()
        {
            //_cacheProvider.Invalidate();
        }
    }
}
