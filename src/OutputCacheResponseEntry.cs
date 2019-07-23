using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace WebEssentials.AspNetCore.OutputCaching
{
    /// <summary>
    /// The cached entry
    /// </summary>
    public class OutputCacheResponseEntry
    {
        private OutputCacheProfile _profile;
        private Dictionary<string, OutputCacheResponse> _responses = new Dictionary<string, OutputCacheResponse>();

        /// <summary>
        /// Creates a new instance of the entry.
        /// </summary>
        public OutputCacheResponseEntry(HttpContext context, byte[] body, OutputCacheProfile profile)
        {
            _profile = profile;
            AddResponse(context, new OutputCacheResponse(body, context.Response.Headers));
        }

        internal void AddResponse(HttpContext context, OutputCacheResponse response)
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

            if (!string.IsNullOrEmpty(_profile.VaryByParam))
            {
                foreach (string param in _profile.VaryByParam.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (param == "*" || context.Request.Query.ContainsKey(param))
                    {
                        key += param + "=" + context.Request.Query[param];
                    }
                }
            }

            if (!string.IsNullOrEmpty(_profile.VaryByHeader))
            {
                foreach (string header in _profile.VaryByHeader.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (header == "*" ||context.Request.Headers.ContainsKey(header))
                    {
                        key += header + "=" + context.Request.Headers[header];
                    }
                }
            }

            if (!string.IsNullOrEmpty(_profile.VaryByCustom))
            {
                var varyByCustomService = context.RequestServices.GetService<IOutputCacheVaryByCustomService>();

                if (varyByCustomService != null)
                {
                    foreach (string argument in _profile.VaryByCustom.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
                    {
                        key += argument + "=" + varyByCustomService.GetVaryByCustomString(argument);
                    }
                }
            }

            return key;
        }
    }
}