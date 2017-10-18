using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// Enables server-side output caching.
    /// </summary>
    public class OutputCacheAttribute : ActionFilterAttribute
    {
        private string[] _fileDependencies;

        /// <summary>
        /// Enables server-side output caching.
        /// </summary>
        /// <param name="fileDependencies">Globbing patterns relative to the content root (not the wwwroot).</param>
        public OutputCacheAttribute(params string[] fileDependencies)
        {
            _fileDependencies = fileDependencies;
        }

        /// <summary>
        /// The duration in seconds of how long to cache the response.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Comma separated list of HTTP headers to vary the caching by.
        /// </summary>
        public string VaryByHeader { get; set; }

        /// <summary>
        /// Comma separated list of query string parameters to vary the caching by.
        /// </summary>
        public string VaryByParam { get; set; }

        /// <summary>
        /// Executing the filter
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.EnableOutputCaching
            (
                slidingExpiration: TimeSpan.FromSeconds(Duration),
                varyByHeaders: VaryByHeader,
                varyByParam: VaryByParam
            );
        }
    }
}
