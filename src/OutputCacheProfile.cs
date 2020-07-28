using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Net.Http.Headers;

namespace WebEssentials.AspNetCore.OutputCaching
{
    /// <summary>
    /// A cache profile.
    /// </summary>
    public class OutputCacheProfile
    {
        /// <summary>
        /// A flag for caching the response based on the user
        /// </summary>
        public bool IsUserBased { get; set; }
        
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
        /// Comma separated list of arguments to vary the caching by using a custom function.
        /// </summary>
        public string VaryByCustom { get; set; }

        /// <summary>
        /// Globbing patterns relative to the content root (not the wwwroot).
        /// </summary>
        public IEnumerable<string> FileDependencies { get; set; } = new[] { "**/*.*" };

        /// <summary>
        /// Use absolute expiration instead of the default sliding expiration.
        /// </summary>
        public bool UseAbsoluteExpiration { get; set; }
        
        /// <summary>
        /// Builds an instance of <see cref="MemoryCacheEntryOptions"/> based on the current configuration.
        /// </summary>
        /// <param name="hostingEnvironment">HostingEnvironment used to watch <see cref="FileDependencies"/>.</param>
        public MemoryCacheEntryOptions BuildMemoryCacheEntryOptions(IHostingEnvironment hostingEnvironment)
        {
            var options = new MemoryCacheEntryOptions();

            var durationTimeSpan = TimeSpan.FromSeconds(Duration);
            if (UseAbsoluteExpiration)
            {
                options.SetAbsoluteExpiration(durationTimeSpan);
            }
            else
            {
                options.SetSlidingExpiration(durationTimeSpan);
            }

            foreach (string globs in FileDependencies)
            {
                options.AddExpirationToken(hostingEnvironment.ContentRootFileProvider.Watch(globs));
            }

            return options;
        }
    }
}