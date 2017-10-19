using System.Collections.Generic;
using Microsoft.Net.Http.Headers;

namespace WebEssentials.AspNetCore.OutputCaching
{
    /// <summary>
    /// A cache profile.
    /// </summary>
    public class OutputCacheProfile
    {
        /// <summary>
        /// The duration in seconds of how long to cache the response.
        /// </summary>
        public double Duration { get; set; }

        /// <summary>
        /// Comma separated list of HTTP headers to vary the caching by.
        /// </summary>
        public string VaryByHeader { get; set; } = HeaderNames.AcceptEncoding;

        /// <summary>
        /// Comma separated list of query string parameters to vary the caching by.
        /// </summary>
        public string VaryByParam { get; set; }

        /// <summary>
        /// Globbing patterns relative to the content root (not the wwwroot).
        /// </summary>
        public IEnumerable<string> FileDependencies { get; set; } = new[] { "**/*.*" };
    }
}
