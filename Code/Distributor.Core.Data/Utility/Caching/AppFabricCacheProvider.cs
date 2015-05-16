using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Distributr.Core.Utility.Caching;
using Microsoft.ApplicationServer.Caching;

namespace Distributr.Core.Data.Utility.Caching
{
    public class AppFabricCacheProvider : ICacheProvider
    {
        private AppFabricCacheProvider()
        {

        }
        private static object locker = new object();
        private static AppFabricCacheProvider instance;
        private static DataCache cache;
        public static AppFabricCacheProvider GetInstance()
        {
            lock (locker)
            {
                if (instance == null)
                {
                    var configuration = new DataCacheFactoryConfiguration();
                    instance = new AppFabricCacheProvider();
                    configuration.SecurityProperties = new DataCacheSecurity(DataCacheSecurityMode.None, DataCacheProtectionLevel.None);
                    DataCacheFactory factory = new DataCacheFactory(configuration);
                    string cacheName = ConfigurationSettings.AppSettings["CacheName"];
                    cache = factory.GetCache(cacheName);
                }
            }
            return instance;
        }
        public void Put(string key, object value)
        {
            cache.Put(key, value);
        }

        public object Get(string key)
        {
            return cache.Get(key);
        }

        public void Remove(string key)
        {
            cache.Remove(key);
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
