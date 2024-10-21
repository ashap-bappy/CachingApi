namespace CachingApi.Interfaces
{
    public interface ICacheService
    {
        Task SetAsync<T>(string key, T value, TimeSpan? expirationTime = null);
        Task<T?> GetAsync<T>(string key);
        Task RemoveAsync(string key);
    }
}
