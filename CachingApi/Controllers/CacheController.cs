using CachingApi.Interfaces;
using CachingApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CachingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly ILogger<CacheController> _logger;
        private readonly ICacheService _cacheService;

        public CacheController(ICacheService cacheService, ILogger<CacheController> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        // GET: api/Cache/5
        [HttpGet("{key}")]
        public async Task<IActionResult> Get(string key)
        {
            _logger.LogInformation($"Received request to get cache for key: {key}");
            var user = await _cacheService.GetAsync<User>(key);
            if (user == null)
            {
                _logger.LogWarning($"Cache miss for key: {key}");
                return NotFound();
            }
            _logger.LogInformation($"Cache hit for key: {key}, returning cached user.");
            return Ok(user);
        }

        // POST: api/Cache
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CacheRequest request)
        {
            if (request.User == null || string.IsNullOrWhiteSpace(request.Key))
            {
                _logger.LogError($"Invalid POST request: user or key is null");
                return BadRequest("Key and User cannot be null or empty.");
            }
            _logger.LogInformation($"Caching user for key: {request.Key} with expiration time of 5 minutes.");
            await _cacheService.SetAsync(request.Key, request.User, TimeSpan.FromMinutes(5));

            _logger.LogInformation($"Successfully cached user for key: {request.Key}");
            return Ok();
        }

        // DELETE: api/Cache/5
        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete(string key)
        {
            _logger.LogInformation($"Received request to delete cache for key: {key}");
            if (string.IsNullOrWhiteSpace(key))
            {
                _logger.LogError("Key cannot be null or empty.");
                return BadRequest("Key cannot be null or empty.");
            }

            await _cacheService.RemoveAsync(key);
            _logger.LogInformation($"Successfully deleted cache for key: {key}");
            return Ok();
        }
    }
}
