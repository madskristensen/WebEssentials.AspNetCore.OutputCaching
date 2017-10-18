using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace WebEssentials.AspNetCore.OutputCaching
{
    /// <summary>
    /// The cached entry
    /// </summary>
    public class OutputCacheResponseEntry
    {
        private OutputCacheFeature _feature;
        private Dictionary<string, OutputCacheResponse> _responses = new Dictionary<string, OutputCacheResponse>();

        /// <summary>
        /// Creates a new instance of the entry.
        /// </summary>
        public OutputCacheResponseEntry(HttpContext context, byte[] body, OutputCacheFeature feature)
        {
            _feature = feature;
            Set(context, new OutputCacheResponse(body, context.Response.Headers));
        }

        internal void Set(HttpContext context, OutputCacheResponse response)
        {
            string key = GetCacheKey(context);
            _responses[key] = response;
        }

        internal bool IsCached(HttpContext context, out OutputCacheResponse response)
        {
            string key = GetCacheKey(context);

            return _responses.TryGetValue(key, out response);
        }

        private string GetCacheKey(HttpContext context)
        {
            string key = "/";

            foreach (string param in _feature.VaryByParam)
            {
                if (context.Request.Query.ContainsKey(param))
                {
                    key += param + "=" + context.Request.Query[param];
                }
            }

            foreach (string header in _feature.VaryByHeaders)
            {
                if (context.Request.Headers.ContainsKey(header))
                {
                    key += header + "=" + context.Request.Headers[header];
                }
            }

            return key;
        }
    }
}