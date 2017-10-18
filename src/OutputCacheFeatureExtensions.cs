using System;
using System.Linq;
using WebEssentials.AspNetCore.OutputCaching;

namespace Microsoft.AspNetCore.Http
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
        public static void EnableOutputCaching(this HttpContext context, IOutputCacheProfile profile)
        {
            var slidingExpiration = TimeSpan.FromSeconds(profile.Duration);
            string varyByHeader = profile.VaryByHeader;
            string varyByParam = profile.VaryByParam;

            context.EnableOutputCaching(slidingExpiration, varyByHeader, varyByParam);
        }

        /// <summary>
        /// Enabled output caching of the response.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="slidingExpiration">The amount of seconds to cache the output for.</param>
        /// <param name="varyByHeaders">Comma separated list of HTTP headers to vary the caching by.</param>
        /// <param name="varyByParam">Comma separated list of query string parameter names to vary the caching by.</param>
        public static void EnableOutputCaching(this HttpContext context, TimeSpan slidingExpiration, string varyByHeaders = null, string varyByParam = null)
        {
            OutputCacheFeature feature = context.Features.Get<OutputCacheFeature>();

            if (feature == null)
            {
                feature = new OutputCacheFeature();
                context.Features.Set(feature);
            }

            feature.SlidingExpiration = slidingExpiration;

            if (varyByHeaders != null)
            {
                feature.VaryByHeaders.AddRange(varyByHeaders.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(h => h.Trim()));
            }

            if (varyByParam != null)
            {
                feature.VaryByParam.AddRange(varyByParam.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()));
            }
        }

        internal static bool IsOutputCachingEnabled(this HttpContext context, out OutputCacheFeature feature)
        {
            feature = context.Features.Get<OutputCacheFeature>();

            return feature != null && feature.IsEnabled;
        }
    }
}
