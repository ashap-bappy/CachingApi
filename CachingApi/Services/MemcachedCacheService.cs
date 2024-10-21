using CachingApi.Helpers;
using CachingApi.Interfaces;
using Enyim.Caching;
using System.Text.Json;

namespace CachingApi.Services
{
    public class MemcachedCacheService : ICacheService
    {
        private readonly IMemcachedClient _memcachedClient;
        private readonly TimeSpan _defaultExpirationTime;

        public MemcachedCacheService(IMemcachedClient memcachedClient, IConfiguration configuration)
        {
            _memcachedClient = memcachedClient;

            var defaultExpirationTimeInMinute = configuration.GetValue<int>("CacheSettings:Memcached:DefaultExpirationTime", 5); // Default to 5 minutes
            if (defaultExpirationTimeInMinute <= 0)
            {
                throw new ArgumentException("Default expiration time must be greater than zero.");
            }
            _defaultExpirationTime = TimeSpan.FromMinutes(defaultExpirationTimeInMinute);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null)
        {
            CacheServiceHelper.ValidateKey(key);

            expirationTime ??= _defaultExpirationTime;
            try
            {
                var jsonValue = JsonSerializer.Serialize(value, CacheServiceHelper.GetSerializerOptions());
                bool success = await _memcachedClient.SetAsync(key, jsonValue, expirationTime.Value);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Error serializing value for key {key}.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while setting cache for key {key}.", ex);
            }
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            CacheServiceHelper.ValidateKey(key);

            try
            {
                var cachedValue = await _memcachedClient.GetAsync<string>(key);

                if (cachedValue == null || string.IsNullOrEmpty(cachedValue.Value))
                {
                    return default;
                }

                return JsonSerializer.Deserialize<T>(cachedValue.Value, CacheServiceHelper.GetSerializerOptions());
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while retrieving cache for key {key}.", ex);
            }
        }

        public async Task RemoveAsync(string key)
        {
            CacheServiceHelper.ValidateKey(key);

            try
            {
                await _memcachedClient.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while removing cache for key {key}.", ex);
            }
        }
    }
}
