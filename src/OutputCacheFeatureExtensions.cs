using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace WebEssentials.AspNetCore.OutputCaching
{
    /// <summary>
    /// Extensions methods for HttpContext
    /// </summary>
    public static class OutputCacheFeatureExtensions
    {
        /// <summary>
        /// Enabled output caching of the response.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="profile">The caching profile to use.</param>
        public static void EnableOutputCaching(this HttpContext context, OutputCacheProfile profile)
        {
            var slidingExpiration = TimeSpan.FromSeconds(profile.Duration);
            string varyByHeader = profile.VaryByHeader;
            string varyByParam = profile.VaryByParam;
            string[] fileDependencies = profile.FileDependencies.ToArray();

            context.EnableOutputCaching(slidingExpiration, varyByHeader, varyByParam, fileDependencies);
        }

        /// <summary>
        /// Enabled output caching of the response.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="slidingExpiration">The amount of seconds to cache the output for.</param>
        /// <param name="varyByHeaders">Comma separated list of HTTP headers to vary the caching by.</param>
        /// <param name="varyByParam">Comma separated list of query string parameter names to vary the caching by.</param>
        /// <param name="fileDependencies">Globbing patterns</param>
        public static void EnableOutputCaching(this HttpContext context, TimeSpan slidingExpiration, string varyByHeaders = null, string varyByParam = null, params string[] fileDependencies)
        {
            OutputCacheProfile feature = context.Features.Get<OutputCacheProfile>();

            if (feature == null)
            {
                feature = new OutputCacheProfile();
                context.Features.Set(feature);
            }

            feature.Duration = slidingExpiration.TotalSeconds;
            feature.FileDependencies = fileDependencies;
            feature.VaryByHeader = varyByHeaders;
            feature.VaryByParam = varyByParam;
        }

        internal static bool IsOutputCachingEnabled(this HttpContext context, out OutputCacheProfile profile)
        {
            profile = context.Features.Get<OutputCacheProfile>();

            return profile != null && profile.Duration > 0;
        }
    }
}
