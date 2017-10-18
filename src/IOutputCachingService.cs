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
        /// <param name="route">The route (usually the same as HttpRequest.Path).</param>
        /// <param name="value">The entry if a match was found in the cache.</param>
        bool TryGetValue(string route, out OutputCacheResponseEntry value);

        /// <summary>
        /// Adds an entry to the cache.
        /// </summary>
        /// <param name="route">The route (usually the same as HttpRequest.Path).</param>
        /// <param name="entry">The entry to add to the cache.</param>
        /// <param name="context">The current HttpContext.</param>
        void Set(string route, OutputCacheResponseEntry entry, HttpContext context);

        /// <summary>
        /// The route to remove from the cache.
        /// </summary>
        /// <param name="route">The route (usually the same as HttpRequest.Path).</param>
        void Remove(string route);

        /// <summary>
        /// Clears the cache.
        /// </summary>
        void Clear();
    }
}