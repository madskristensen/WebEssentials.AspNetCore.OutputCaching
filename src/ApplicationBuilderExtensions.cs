using Microsoft.AspNetCore.Builder;

namespace WebEssentials.AspNetCore.OutputCaching
{
    /// <summary>
    /// Extensions for registering the output caching middleware.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Registers the output caching middleware
        /// </summary>
        public static void UseOutputCaching(this IApplicationBuilder app)
        {
            app.UseMiddleware<OutputCacheMiddleware>();
        }
    }
}