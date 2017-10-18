using WebEssentials.AspNetCore.OutputCaching;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddOutputCaching(this IServiceCollection services)
        {
            services.AddSingleton<IOutputCachingService, OutputCachingService>();
        }
    }
}