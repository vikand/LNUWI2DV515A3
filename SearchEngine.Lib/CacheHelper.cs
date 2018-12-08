using System.Runtime.Caching;

namespace SearchEngine.Lib
{
    public class CacheHelper : ICacheHelper
    {
        private readonly ObjectCache _objectCache;

        public CacheHelper()
        {
            _objectCache = MemoryCache.Default;
        }

        public T Get<T>(string key) where T : class => _objectCache[key] as T;

        public void Set<T>(string key, T item, CacheItemPolicy cacheItemPolicy) => 
            _objectCache.Set(key, item, cacheItemPolicy);

        public bool Contains(string key) => _objectCache.Contains(key);
    }
}
