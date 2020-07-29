﻿using System;
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
            bool isUserBase = profile.IsUserBased;
            var slidingExpiration = TimeSpan.FromSeconds(profile.Duration);
            string varyByHeader = profile.VaryByHeader;
            string varyByParam = profile.VaryByParam;
            string varyByCustom = profile.VaryByCustom;
            string[] fileDependencies = profile.FileDependencies.ToArray();
            bool useAbsoluteExpiration = profile.UseAbsoluteExpiration;

            context.EnableOutputCaching(slidingExpiration, varyByHeader, varyByParam, varyByCustom, useAbsoluteExpiration, isUserBase, fileDependencies);
        }

        /// <summary>
        /// Enabled output caching of the response.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="slidingExpiration">The amount of seconds to cache the output for.</param>
        /// <param name="varyByHeaders">Comma separated list of HTTP headers to vary the caching by.</param>
        /// <param name="varyByParam">Comma separated list of query string parameter names to vary the caching by.</param>
        /// <param name="varyByCustom">Comma separated list of arguments to vary the caching by using a custom function.</param>
        /// <param name="useAbsoluteExpiration">Use absolute expiration instead of the default sliding expiration.</param>
        /// <param name="isUserBase">A flag for caching the response based on the user</param>
        /// <param name="fileDependencies">Globbing patterns</param>
        public static void EnableOutputCaching(this HttpContext context, TimeSpan slidingExpiration, string varyByHeaders = null, string varyByParam = null, string varyByCustom = null, bool useAbsoluteExpiration = false, bool isUserBase = true, params string[] fileDependencies)
        {
            OutputCacheProfile feature = context.Features.Get<OutputCacheProfile>();

            if (feature == null)
            {
                feature = new OutputCacheProfile();
                context.Features.Set(feature);
            }

            feature.IsUserBased = isUserBase;
            feature.Duration = slidingExpiration.TotalSeconds;
            feature.FileDependencies = fileDependencies;
            feature.VaryByHeader = varyByHeaders;
            feature.VaryByParam = varyByParam;
            feature.VaryByCustom = varyByCustom;
            feature.UseAbsoluteExpiration = useAbsoluteExpiration;
        }

        internal static bool IsOutputCachingEnabled(this HttpContext context)
        {
            return context.IsOutputCachingEnabled(out OutputCacheProfile _);
        }

        internal static bool IsOutputCachingEnabled(this HttpContext context, out OutputCacheProfile profile)
        {
            profile = context.Features.Get<OutputCacheProfile>();

            return profile != null && profile.Duration > 0;
        }
    }
}