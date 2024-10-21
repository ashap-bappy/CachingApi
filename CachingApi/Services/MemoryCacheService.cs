using CachingApi.Helpers;
using CachingApi.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace CachingApi.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null)
        {
            CacheServiceHelper.ValidateKey(key);
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expirationTime ?? TimeSpan.FromMinutes(5)
            });
            return Task.CompletedTask;
        }

        public Task<T?> GetAsync<T>(string key)
        {
            CacheServiceHelper.ValidateKey(key);
            var value = _memoryCache.Get(key);
            return Task.FromResult((T?)value);
        }

        public Task RemoveAsync(string key)
        {
            CacheServiceHelper.ValidateKey(key);
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
