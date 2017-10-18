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
            Profiles = new Dictionary<string, IOutputCacheProfile>();
        }

        /// <summary>
        /// Determines if the middleware is enabled for the specified HTTP context.
        /// </summary>
        public Func<HttpContext, bool> IsRequestValid { get; set; } = (context) => { return true; };

        /// <summary>
        /// A list of named caching profiles.
        /// </summary>
        public IDictionary<string, IOutputCacheProfile> Profiles { get; }

        internal Func<HttpContext, bool> DefaultContextCheck = (context) =>
        {
            if (context.Request.Method != HttpMethods.Get) return false;
            if (context.Response.StatusCode != StatusCodes.Status200OK) return false;
            if (context.User.Identity.IsAuthenticated) return false;
            if (context.Response.Headers.ContainsKey(HeaderNames.SetCookie)) return false;

            return true;
        };
    }
}
