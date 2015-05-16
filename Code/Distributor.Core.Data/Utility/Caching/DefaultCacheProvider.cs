using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using Distributr.Core.Utility.Caching;

namespace Distributr.Core.Data.Utility.Caching
{
    /// <summary>
    /// NB THIS CLASS SHOULD NOT BE USED IN A DISTRIBUTED PRODUCTION ENVIRONMENT
    /// OR PROBABLY IN ANY PRODUCTION ENVIRONMENT
    /// </summary>
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
                    cache =MemoryCache.Default;
                }
            }
            return instance;
        }
        public void Put(string key, object value)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(60);
            cache.Set(new CacheItem(key, value), policy);
        }

        public object Get(string key)
        {
            return cache.Get(key);
        }

        public void Remove(string key)
        {
            var item = cache.Get(key);
            cache.Remove(key);
        }

        public void Reset()
        {
            foreach(var item in cache)
            {
                cache.Remove(item.Key);
            }
        }

        //NB MemoryCache does not implement regions so need to do regions manually


        //private bool _enableCaching = false;
        
        //string cacheEntryTemplate = "region_{0}_key_{1}";
        ////public DefaultCacheProvider(bool enableCaching)
        //public DefaultCacheProvider()
        //{
        //    _enableCaching = true;
        //}

        //private ObjectCache Cache { get { return MemoryCache.Default; } }

        //public object Get(string regionName, string key)
        //{
        //    if (!_enableCaching)
        //        return false;
        //    string cacheKey = CacheKey(regionName, key);
        //    return Cache.Get(cacheKey);
        //}

        //private string CacheKey(string regionName, string key)
        //{
        //    string cacheKey = string.Format(cacheEntryTemplate, regionName, key);
        //    return cacheKey;
        //}

        //public void Set(string regionName, string key, object data, int cacheTime)
        //{
        //    if (!_enableCaching)
        //        return;
        //    string cacheKey = CacheKey(regionName, key);
        //    CacheItemPolicy policy = new CacheItemPolicy();
        //    policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime);

        //    Cache.Add(new CacheItem(cacheKey, data), policy);
        //}

        //public bool IsSet(string regionName, string key)
        //{
        //    if (!_enableCaching)
        //        return false;
        //    //string cacheKey = CacheKey(regionName, key);
        //    return Get( regionName, key) == null;
        //}

        //public bool Invalidate(string regionName, string key)
        //{
        //    if (!_enableCaching)
        //        return false;
        //    string cacheKey = CacheKey(regionName, key);
        //    object o = Cache.Remove(cacheKey);
        //    return o != null;

        //}

        //public void InvalidateRegion(string regionName)
        //{
        //    if (!_enableCaching)
        //        return;
        //    string[] keys = Cache.Select(n => n.Key).ToArray();
        //    foreach (string k in keys)
        //    {
        //        if (k.StartsWith("region_" + regionName))
        //            Cache.Remove(k);
        //    }

        //}

        //#region ICacheProvider Members


        //public void Invalidate()
        //{
        //    if (!_enableCaching)
        //        return;
        //    string[] keys = Cache.Select(n => n.Key).ToArray();
        //    foreach (string k in keys)
        //    {
        //        Cache.Remove(k);
        //    }
        //}

        //#endregion
       
    }
}
