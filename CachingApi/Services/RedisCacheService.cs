using System.Text.Json;
using CachingApi.Helpers;
using CachingApi.Interfaces;
using StackExchange.Redis;

namespace CachingApi.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _redisDatabase;
        private readonly TimeSpan _defaultExpirationTime;

        public RedisCacheService(IConnectionMultiplexer redis, IConfiguration configuration)
        {
            _redisDatabase = redis.GetDatabase();

            var defaultExpirationTimeInMinute = configuration.GetValue<int>("CacheSettings:Redis:DefaultExpirationTime", 5); // Default to 5 minutes
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
                await _redisDatabase.StringSetAsync(key, jsonValue, expirationTime);
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
                string? value = await _redisDatabase.StringGetAsync(key);

                if (string.IsNullOrEmpty(value))
                {
                    return default;
                }

                return JsonSerializer.Deserialize<T>(value, CacheServiceHelper.GetSerializerOptions());
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
                await _redisDatabase.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while removing cache for key {key}.", ex);
            }
        }
    }
}
