using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

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
        
        public bool TryGetValue(HttpContext context, JObject jObject, out OutputCacheResponse response)
        {
            response = null;
            return
                _cache.TryGetValue(_cacheKeysProvider.GetCacheProfileCacheKey(context.Request), out OutputCacheProfile profile) &&
                _cache.TryGetValue(_cacheKeysProvider.GetRequestCacheKey(context, profile, jObject), out response);
        }

        public void Set(HttpContext context, OutputCacheResponse response, JObject jObject)
        {
            if (!context.IsOutputCachingEnabled(out OutputCacheProfile profile)) 
                return;
            
            AddProfileToCache(context, profile);
            AddResponseToCache(context, profile, response, jObject);
        }

        public string GetRequestCacheKey(HttpContext context, OutputCacheProfile profile, JObject jObject)
        {
            return _cacheKeysProvider.GetRequestCacheKey(context, profile, jObject);
        }

        private void AddProfileToCache(HttpContext context, OutputCacheProfile profile)
        {
            var profileCacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheOptions.ProfileCacheDuration
            };
            _cache.Set(_cacheKeysProvider.GetCacheProfileCacheKey(context.Request), profile, profileCacheEntryOptions);
        }

        private void AddResponseToCache(HttpContext context, OutputCacheProfile profile, OutputCacheResponse response,
            JObject jObject)
        {
            IHostingEnvironment hostingEnvironment = context.RequestServices.GetRequiredService<IHostingEnvironment>();
            MemoryCacheEntryOptions options = profile.BuildMemoryCacheEntryOptions(hostingEnvironment);
            _cache.Set(_cacheKeysProvider.GetRequestCacheKey(context, profile, jObject), response, options);
        }

        public void Clear()
        {
            _cache?.Dispose();

            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public void Remove(string method, string host, string path)
        {
            FieldInfo fieldInfo = _cache.GetType().GetField("_entries", BindingFlags.Instance | BindingFlags.NonPublic);
            object entries = fieldInfo?.GetValue(_cache);

            var keys = (IReadOnlyCollection<object>) entries?.GetType().GetProperty("Keys")?.GetValue(entries);

            if (keys == null)
                return;

            foreach (object key in keys.Where(key => ((string) key).StartsWith($"{method}_{host}{path}")))
                _cache.Remove(key);
        }
    }
}