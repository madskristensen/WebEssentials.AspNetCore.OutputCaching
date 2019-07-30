using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace WebEssentials.AspNetCore.OutputCaching
{
    /// <summary>
    /// Options for the output caching service.
    /// </summary>
    public class OutputCacheOptions
    {
        internal OutputCacheOptions()
        {
            Profiles = new Dictionary<string, OutputCacheProfile>();
        }

        /// <summary>
        /// Determines if the HTTP request is appropriate for the middleware to engage.
        /// </summary>
        public Func<HttpContext, bool> DoesRequestQualify { get; set; } = DefaultRequestQualifier;

        /// <summary>
        /// Determines if the HTTP response from MVC is appropriate for the middleware to cache.
        /// </summary>
        public Func<HttpContext, bool> DoesResponseQualify { get; set; } = DefaultResponseQualifier;

        /// <summary>
        /// A list of named caching profiles.
        /// </summary>
        public IDictionary<string, OutputCacheProfile> Profiles { get; }

        /// <summary>
        /// Defines how long <see cref="OutputCacheProfile"/>s are cached.
        /// </summary>
        public TimeSpan ProfileCacheDuration { get; set; } = TimeSpan.FromDays(1);

        /// <summary>
        /// The default request validator used by the middleware.
        /// </summary>
        public static Func<HttpContext, bool> DefaultRequestQualifier = (context) =>
        {
            if (context.Request.Method != HttpMethods.Get) return false;
            if (context.User.Identity.IsAuthenticated) return false;

            return true;
        };

        /// <summary>
        /// The default response validator used by the middleware.
        /// </summary>
        public static Func<HttpContext, bool> DefaultResponseQualifier = (context) =>
        {
            if (context.Response.StatusCode != StatusCodes.Status200OK) return false;
            if (context.Response.Headers.ContainsKey(HeaderNames.SetCookie)) return false;

            return true;
        };
    }
}
