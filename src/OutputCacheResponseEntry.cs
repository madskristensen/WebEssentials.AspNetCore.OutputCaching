using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace WebEssentials.AspNetCore.OutputCaching
{
    public class OutputCacheResponseEntry
    {
        private Dictionary<string, OutputCacheResponse> _responses = new Dictionary<string, OutputCacheResponse>();

        public OutputCacheResponseEntry(HttpContext context, byte[] body, OutputCacheFeature feature)
        {
            Feature = feature;
            Set(context, new OutputCacheResponse(body, context.Response.Headers));
        }

        public OutputCacheFeature Feature { get; set; }

        public void Set(HttpContext context, OutputCacheResponse response)
        {
            string key = GetCacheKey(context);
            _responses[key] = response;
        }


        public bool IsCached(HttpContext context, out OutputCacheResponse response)
        {
            string key = GetCacheKey(context);

            return _responses.TryGetValue(key, out response);
        }

        private string GetCacheKey(HttpContext context)
        {
            string key = "/";

            foreach (string param in Feature.VaryByParam)
            {
                if (context.Request.Query.ContainsKey(param))
                {
                    key += param + "=" + context.Request.Query[param];
                }
            }

            foreach (string header in Feature.VaryByHeaders)
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