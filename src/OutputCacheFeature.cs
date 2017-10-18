using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;

namespace WebEssentials.AspNetCore.OutputCaching
{
    /// <summary>
    /// The HTTP context feature that enables output caching.
    /// </summary>
    public class OutputCacheFeature
    {
        /// <summary>
        /// Creates a new instance of the feature.
        /// </summary>
        public OutputCacheFeature()
        {
            FileDependencies = new List<string>();
            VaryByHeaders = new List<string>(new[] { HeaderNames.AcceptEncoding });
            VaryByParam = new List<string>();
        }

        /// <summary>
        /// A list of file globbing patterns relative to the content root of the app (not wwwroot).
        /// </summary>
        public List<string> FileDependencies { get; }

        /// <summary>
        /// The sliding expiration of the cached response.
        /// </summary>
        public TimeSpan? SlidingExpiration { get; set; }

        /// <summary>
        /// List of HTTP headers to vary the caching by.
        /// </summary>
        public List<string> VaryByHeaders { get; }

        /// <summary>
        /// List of query string parameters to vary the caching by.
        /// </summary>
        public List<string> VaryByParam { get; }

        internal bool IsEnabled
        {
            get { return SlidingExpiration.HasValue; }
        }
    }
}
