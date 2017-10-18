using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using WebEssentials.AspNetCore.OutputCaching;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Extensions for registering the output caching middleware.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        private static Func<HttpContext, bool> IsCachingSupported = (context) =>
        {
            if (context.Request.Method != HttpMethods.Get) return false;
            if (context.Response.StatusCode != StatusCodes.Status200OK) return false;
            if (context.User.Identity.IsAuthenticated) return false;
            if (context.Response.Headers.ContainsKey(HeaderNames.SetCookie)) return false;

            return true;
        };

        /// <summary>
        /// Registers the output caching middleware
        /// </summary>
        public static void UseOutputCaching(this IApplicationBuilder app)
        {
            app.UseMiddleware<OutputCacheMiddleware>(IsCachingSupported);
        }

        /// <summary>
        /// Registers the output caching middleware
        /// </summary>
        /// <param name="filter">A filter function that determines if the response supports output caching.</param>
        public static void UseOutputCaching(this IApplicationBuilder app, Func<HttpContext, bool> filter)
        {
            app.UseMiddleware<OutputCacheMiddleware>(filter);
        }
    }
}