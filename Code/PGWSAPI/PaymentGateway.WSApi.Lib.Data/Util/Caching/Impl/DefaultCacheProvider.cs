using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace PaymentGateway.WSApi.Lib.Data.Util.Caching.Impl
{
    public class DefaultCacheProvider : ICacheProvider
    {
        private DefaultCacheProvider()
        {

        }
        private static object locker = new object();
        private static DefaultCacheProvider instance;
        private static ObjectCache cache;
        public static DefaultCacheProvider GetInstance()
        {
            lock (locker)
            {
                if (instance == null)
                {
                    instance = new DefaultCacheProvider();
                    cache = MemoryCache.Default;
                }
            }
            return instance;
        }
        public void Put(string key, object value)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(360);
            cache.Set(new CacheItem(key, value), policy);
        }

        public object Get(string key)
        {
            return cache.Get(key);
        }

        public void Remove(string key)
        {
            cache.Remove(key);
        }
    }
}
