using CachingApi.Interfaces;
using CachingApi.Services;
using StackExchange.Redis;

namespace CachingApi.Extensions
{
    public static class CacheServiceExtension
    {
        public static IServiceCollection AddCustomCache(this IServiceCollection services, IConfiguration configuration)
        {
            string cacheType = configuration["CacheSettings:CacheType"] ?? "Memory";

            switch (cacheType)
            {
                case "Redis":
                    SetRedisConfig(services, configuration);
                    break;
                case "Memcached":
                    SetMemcachedConfig(services, configuration);
                    break;
                default:
                    SetInMemoryConfig(services);
                    break;
            }
            return services;
        }

        private static void SetInMemoryConfig(IServiceCollection services)
        {
            services.AddSingleton<ICacheService, MemoryCacheService>();
            services.AddMemoryCache();
        }

        private static void SetMemcachedConfig(IServiceCollection services, IConfiguration configuration)
        {
            var server = configuration.GetSection("CacheSettings:Memcached:Server");

            services.AddSingleton<ICacheService, MemcachedCacheService>();
            services.AddEnyimMemcached(options =>
            {
                var logger = services.BuildServiceProvider().GetRequiredService<ILogger<MemcachedCacheService>>();
                try
                {
                    string address = server?.GetValue<string>("Address") ?? "localhost";
                    int port = server?.GetValue<int>("Port") ?? 11211;

                    logger.LogInformation($"Adding Memcached server: {address}:{port}");
                    options.AddServer(address, port);
                    logger.LogInformation("Memcached configuration completed successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to configure Memcached servers.");
                    throw;
                }
            });
        }


        private static void SetRedisConfig(IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetSection("CacheSettings:Redis")?.GetValue<string>("ConnectionString") ?? "localhost:6379";

            services.AddSingleton<ICacheService, RedisCacheService>();
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<RedisCacheService>>();

                try
                {
                    logger.LogInformation("Attempting to connect to Redis...");
                    return ConnectionMultiplexer.Connect(connectionString);
                }
                catch (RedisConnectionException ex)
                {
                    logger.LogError(ex, "Failed to connect to Redis using the provided connection string");
                    throw new Exception("Failed to connect to Redis", ex);
                }
            });
        }
    }
}
