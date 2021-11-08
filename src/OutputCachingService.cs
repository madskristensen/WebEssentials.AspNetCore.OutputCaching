using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace WebEssentials.AspNetCore.OutputCaching
{
    internal class OutputCachingService : IOutputCachingService
    {
        private IMemoryCache _cache;
        private readonly IOutputCacheKeysProvider _cacheKeysProvider;
        private readonly OutputCacheOptions _cacheOptions;

        public OutputCachingService(
            IOutputCacheKeysProvider cacheKeysProvider,
            OutputCacheOptions cacheOptions)
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _cacheKeysProvider = cacheKeysProvider;
            _cacheOptions = cacheOptions;
        }

        public bool TryGetValue(HttpContext context, out OutputCacheResponse response)
        {
            response = null;
            return
                _cache.TryGetValue(_cacheKeysProvider.GetCacheProfileCacheKey(context.Request), out OutputCacheProfile profile) &&
                _cache.TryGetValue(_cacheKeysProvider.GetRequestCacheKey(context, profile), out response);
        }

        public void Set(HttpContext context, OutputCacheResponse response)
        {
            if (context.IsOutputCachingEnabled(out OutputCacheProfile profile))
            {
                AddProfileToCache(context, profile);
                AddResponseToCache(context, profile, response);
            }
        }

        private void AddProfileToCache(HttpContext context, OutputCacheProfile profile)
        {
            var profileCacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheOptions.ProfileCacheDuration
            };
            _cache.Set(_cacheKeysProvider.GetCacheProfileCacheKey(context.Request), profile, profileCacheEntryOptions);
        }

        private void AddResponseToCache(HttpContext context, OutputCacheProfile profile, OutputCacheResponse response)
        {
            var hostingEnvironment = context.RequestServices.GetRequiredService<IHostingEnvironment>();
            var options = profile.BuildMemoryCacheEntryOptions(hostingEnvironment);
            _cache.Set(_cacheKeysProvider.GetRequestCacheKey(context, profile), response, options);
        }

        public void Clear()
        {
            var old = Interlocked.Exchange(ref _cache, new MemoryCache(new MemoryCacheOptions()));
            old?.Dispose();
        }

        public void Remove(string cacheKey)
        {
            _cache.Remove(cacheKey);
        }
    }
}