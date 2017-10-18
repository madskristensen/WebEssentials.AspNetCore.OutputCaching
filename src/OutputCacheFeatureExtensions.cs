using System;
using System.Linq;
using WebEssentials.AspNetCore.OutputCaching;

namespace Microsoft.AspNetCore.Http
{
    public static class OutputCacheFeatureExtensions
    {
        public static void EnableOutputCaching(this HttpContext context, TimeSpan slidingExpiration, string varyByHeaders = null, string varyByParam = null)
        {
            var feature = context.Features.Get<OutputCacheFeature>();

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

        public static void AddFileDependencies(this HttpContext context, params string[] fileDependencies)
        {
            var feature = context.Features.Get<OutputCacheFeature>();

            if (feature != null)
            {
                feature.FileDependencies.AddRange(fileDependencies);
            }
        }

        internal static bool IsOutputCachingEnabled(this HttpContext context, out OutputCacheFeature feature)
        {
            feature = context.Features.Get<OutputCacheFeature>();

            return feature != null && feature.IsEnabled;
        }
    }
}
