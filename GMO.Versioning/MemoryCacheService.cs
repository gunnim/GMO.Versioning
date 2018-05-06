using System.Runtime.Caching;

namespace GMO.Versioning
{
    class MemoryCacheService : IMemoryCacheService
    {
        public MemoryCache Default => MemoryCache.Default;
        public T Get<T>(string key) => (T)Default.Get(key);
    }

    public interface IMemoryCacheService
    {
        MemoryCache Default { get; }
        T Get<T>(string key);
    }
}
