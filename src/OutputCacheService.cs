using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace WebEssentials.AspNetCore.OutputCaching
{
    internal class OutputCachingService : IOutputCachingService
    {
        private IHostingEnvironment _env;
        private IMemoryCache _cache;

        public OutputCachingService(IHostingEnvironment env)
        {
            _env = env;
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public bool TryGetValue(string route, out OutputCacheResponseEntry value)
        {
            string cleanRoute = NormalizeRoute(route);
            return _cache.TryGetValue(cleanRoute, out value);
        }

        public void Set(string route, OutputCacheResponseEntry entry, HttpContext context)
        {
            if (!context.IsOutputCachingEnabled(out OutputCacheFeature feature))
                return;

            var options = new MemoryCacheEntryOptions();
            options.SetSlidingExpiration(feature.SlidingExpiration.Value);

            foreach (string globs in feature.FileDependencies)
            {
                options.AddExpirationToken(_env.ContentRootFileProvider.Watch(globs));
            }

            string cleanRoute = NormalizeRoute(route);
            _cache.Set(cleanRoute, entry, options);
        }

        public void Remove(string route)
        {
            string cleanRoute = NormalizeRoute(route);
            _cache.Remove(cleanRoute);
        }

        public void Clear()
        {
            if (_cache != null)
            {
                _cache.Dispose();
            }

            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        private static string NormalizeRoute(string route)
        {
            return "/" + route.Trim().Trim('/');
        }
    }
}