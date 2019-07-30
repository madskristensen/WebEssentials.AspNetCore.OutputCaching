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
        /// <param name="requestCacheKey">The cache key that can be built using the <see cref="BuildRequestCacheKey(HttpRequest)"/> method.</param>
        /// <param name="value">The entry if a match was found in the cache.</param>
        bool TryGetValue(string requestCacheKey, out OutputCacheResponseEntry value);

        /// <summary>
        /// Adds an entry to the cache.
        /// </summary>
        /// <param name="requestCacheKey">The cache key that can be built using the <see cref="BuildRequestCacheKey(HttpRequest)"/> method.</param>
        /// <param name="entry">The entry to add to the cache.</param>
        /// <param name="context">The current HttpContext.</param>
        void Set(string requestCacheKey, OutputCacheResponseEntry entry, HttpContext context);

        /// <summary>
        /// The route to remove from the cache.
        /// </summary>
        /// <param name="requestCacheKey">The cache key that can be built using the <see cref="BuildRequestCacheKey(HttpRequest)"/> method.</param>
        void Remove(string requestCacheKey);

        /// <summary>
        /// Clears the cache.
        /// </summary>
        void Clear();

        /// <summary>
        /// Builds the cache key for the provided <paramref name="request"/>.
        /// </summary>
        /// <param name="request">The request for which the cache key should be built.</param>
        string BuildRequestCacheKey(HttpRequest request);
    }
}