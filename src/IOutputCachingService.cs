using Microsoft.AspNetCore.Http;

namespace WebEssentials.AspNetCore.OutputCaching
{
    /// <summary>
    /// An interface for the output caching service.
    /// </summary>
    public interface IOutputCachingService
    {
        /// <summary>
        /// Attempts to get the value of a cached response.
        /// </summary>
        /// <param name="context">The current HttpContext.</param>
        /// <param name="response">The cached response if a match was found in the cache.</param>
        bool TryGetValue(HttpContext context, out OutputCacheResponse response);

        /// <summary>
        /// Adds an entry to the cache.
        /// </summary>
        /// <param name="context">The current HttpContext.</param>
        /// <param name="response">The response to cache.</param>
        void Set(HttpContext context, OutputCacheResponse response);
        
        /// <summary>
        /// Clears the cache.
        /// </summary>
        void Clear();

        /// <summary>
        /// Removes entry from the cache
        /// </summary>
        /// <param name="cacheKey">Cache key generated for cached response</param>
        void Remove(string cacheKey);
    }
}