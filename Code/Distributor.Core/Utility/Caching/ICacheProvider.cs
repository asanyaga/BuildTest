namespace Distributr.Core.Utility.Caching
{
    public interface ICacheProvider
    {
        //object Get(string regionName, string key);
        //void Set(string regionName, string key, object data, int cacheTime);
        //bool IsSet(string regionName, string key);
        //bool Invalidate(string regionName, string key);
        //void InvalidateRegion(string regionName);
        //void Invalidate();
        void Put(string key, object value);
        object Get(string key);
        void Remove(string key);
        void Reset();

    }
}
