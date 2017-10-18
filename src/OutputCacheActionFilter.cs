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
        /// <param name="duration">The duration in seconds that the page should stay in the cache.</param>
        /// <param name="fileDependencies">Globbing patterns relative to the content root (not the wwwroot).</param>
        public OutputCacheAttribute(params string[] fileDependencies)
        {
            _fileDependencies = fileDependencies;
        }

        public int Duration { get; set; }
        public string VaryByHeader { get; set; }
        public string VaryByParam { get; set; }

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
