using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Net.Http.Headers;
using WebEssentials.AspNetCore.OutputCaching;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods to register the output caching service.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the output caching service with the dependency injection system.
        /// </summary>
        public static void AddOutputCaching(this IServiceCollection services)
        {
            services.AddSingleton<IOutputCachingService, OutputCachingService>();
        }

        /// <summary>
        /// Registers the output caching service with the dependency injection system.
        /// </summary>
        public static void AddOutputCaching(this IServiceCollection services, Action<OutputCacheOptions> outputCacheOptions)
        {
            var options = new OutputCacheOptions();
            outputCacheOptions(options);

            services.AddSingleton(options);
            services.TryAddSingleton<IOutputCachingService, OutputCachingService>();
        }
    }
}