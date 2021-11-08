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
        /// <param name="forPath">The Path part of HTTP <see cref="HttpRequest"/>.</param>
        /// <param name="httpMethod">HTTP Method of <see cref="HttpRequest"/>.</param>
        string GetCacheProfileCacheKey(HttpRequest request, string forPath = null, string httpMethod = null);

        /// <summary>
        /// Creates a key for caching <see cref="OutputCacheResponse"/> instances depending on a <paramref name="profile"/>.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContext"/>.</param>
        /// <param name="profile">The profile that is used to cache the request.</param>
        /// <param name="forPath">The Path part of HTTP <see cref="HttpRequest"/>.</param>
        /// <param name="httpMethod">HTTP Method of <see cref="HttpRequest"/>.</param>
        string GetRequestCacheKey(HttpContext context, OutputCacheProfile profile, string forPath = null, string httpMethod = null);
    }
}