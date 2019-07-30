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

        public bool TryGetValue(HttpRequest request, out OutputCacheResponseEntry value)
        {
            return TryGetValue(BuildRequestCacheKey(request), out value);
        }

        public bool TryGetValue(string requestCacheKey, out OutputCacheResponseEntry value)
        {
            return _cache.TryGetValue(requestCacheKey, out value);
        }

        public void Set(string requestCacheKey, OutputCacheResponseEntry entry, HttpContext context)
        {
            if (!context.IsOutputCachingEnabled(out OutputCacheProfile profile))
                return;

            var env = (IHostingEnvironment)context.RequestServices.GetService(typeof(IHostingEnvironment));

            var options = new MemoryCacheEntryOptions();
            if (profile.UseAbsoluteExpiration)
            {
                options.SetAbsoluteExpiration(TimeSpan.FromSeconds(profile.Duration));
            }
            else
            {
                options.SetSlidingExpiration(TimeSpan.FromSeconds(profile.Duration));
            }

            foreach (string globs in profile.FileDependencies)
            {
                options.AddExpirationToken(env.ContentRootFileProvider.Watch(globs));
            }

            _cache.Set(requestCacheKey, entry, options);
        }

        public void Remove(string requestCacheKey)
        {
            _cache.Remove(requestCacheKey);
        }

        public void Clear()
        {
            if (_cache != null)
            {
                _cache.Dispose();
            }

            _cache = new MemoryCache(new MemoryCacheOptions());
        }
        
        public string BuildRequestCacheKey(HttpRequest request)
        {
            return $"{request.Method}_{request.Host}{request.Path}";
        }
    }
}