using System.Runtime.Caching;

namespace GMO.Versioning
{
    class MemoryCacheService : IMemoryCacheService
    {
        public MemoryCache Default => MemoryCache.Default;
        public T Get<T>(string key) => (T)Default.Get(key);
    }

    /// <summary>
    /// System.Runtime.Caching abstraction wrapper
    /// </summary>
    public interface IMemoryCacheService
    {
        MemoryCache Default { get; }
        T Get<T>(string key);
    }
}
