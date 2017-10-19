using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace WebEssentials.AspNetCore.OutputCaching
{
    internal class OutputCachingService : IOutputCachingService
    {
        private IMemoryCache _cache;

        public OutputCachingService()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public bool TryGetValue(string route, out OutputCacheResponseEntry value)
        {
            string cleanRoute = NormalizeRoute(route);
            return _cache.TryGetValue(cleanRoute, out value);
        }

        public void Set(string route, OutputCacheResponseEntry entry, HttpContext context)
        {
            if (!context.IsOutputCachingEnabled(out OutputCacheProfile profile))
                return;

            var env = (IHostingEnvironment)context.RequestServices.GetService(typeof(IHostingEnvironment));

            var options = new MemoryCacheEntryOptions();
            options.SetSlidingExpiration(TimeSpan.FromSeconds(profile.Duration));

            foreach (string globs in profile.FileDependencies)
            {
                options.AddExpirationToken(env.ContentRootFileProvider.Watch(globs));
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