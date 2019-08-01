using Microsoft.AspNetCore.Http;

namespace WebEssentials.AspNetCore.OutputCaching
{
    /// <summary>
    /// Provides methods to build cache keys.
    /// </summary>
    public interface IOutputCacheKeysProvider
    {
        /// <summary>
        /// Creates a key for caching the <see cref="OutputCacheProfile"/> for the provided <paramref name="request"/>.
        /// </summary>
        /// <param name="request">The current request.</param>
        string GetCacheProfileCacheKey(HttpRequest request);

        /// <summary>
        /// Creates a key for caching <see cref="OutputCacheResponse"/> instances depending on a <paramref name="profile"/>.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContext"/>.</param>
        /// <param name="profile">The profile that is used to cache the request.</param>
        string GetRequestCacheKey(HttpContext context, OutputCacheProfile profile);
    }
}