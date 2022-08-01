using Microsoft.Extensions.Caching.Memory;
using System;

namespace Lidya.MemoryCaching
{
    public static class LdyMemoryCache
    {
        private static IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public static void Add<TValue>(string key, TValue value, int expiration = 43200)
        {
            var option = new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(expiration),
                Priority = CacheItemPriority.High,
                SlidingExpiration = TimeSpan.FromSeconds(expiration + 5)
            };
            _cache.Set(key, value, option);
        }

        public static TValue Get<TValue>(string key)
        {
            return _cache.Get<TValue>(key);
        }

        public static void Delete(string key)
        {
            _cache.Remove(key);
        }
    }
}
