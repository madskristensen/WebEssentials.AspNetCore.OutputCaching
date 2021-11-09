using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace WebEssentials.AspNetCore.OutputCaching
{
    internal class OutputCacheKeysProvider : IOutputCacheKeysProvider
    {
        public string GetCacheProfileCacheKey(HttpRequest request, string httpMethod = null, string forPath = null)
        {
            return $"{httpMethod ?? request.Method}_{request.Host}{forPath ?? request.Path}";
        }

        public string GetRequestCacheKey(HttpContext context, OutputCacheProfile profile, string httpMethod = null, string forPath = null, IQueryCollection query = null)
        {
            HttpRequest request = context.Request;
            string key = GetCacheProfileCacheKey(request, httpMethod, forPath) + "_";

            if (!string.IsNullOrEmpty(profile.VaryByParam))
            {
                query = query ?? request.Query;
                foreach (string param in profile.VaryByParam.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (param == "*" || query.ContainsKey(param))
                    {
                        key += param + "=" + query[param];
                    }
                }
            }

            if (!string.IsNullOrEmpty(profile.VaryByHeader))
            {
                foreach (string header in profile.VaryByHeader.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (header == "*" || request.Headers.ContainsKey(header))
                    {
                        key += header + "=" + request.Headers[header];
                    }
                }
            }

            if (!string.IsNullOrEmpty(profile.VaryByCustom))
            {
                var varyByCustomService = context.RequestServices.GetService<IOutputCacheVaryByCustomService>();

                if (varyByCustomService != null)
                {
                    foreach (string argument in profile.VaryByCustom.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        key += argument + "=" + varyByCustomService.GetVaryByCustomString(context, argument);
                    }
                }
            }

            return key.ToLowerInvariant();
        }
    }
}