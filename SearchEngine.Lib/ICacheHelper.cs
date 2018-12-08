using System.Runtime.Caching;

namespace SearchEngine.Lib
{
    public interface ICacheHelper
    {
        T Get<T>(string key) where T : class;
        void Set<T>(string key, T item, CacheItemPolicy cacheItemPolicy);
        bool Contains(string key);
    }
}