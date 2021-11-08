using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace WebEssentials.AspNetCore.OutputCaching
{
    internal class OutputCacheKeysProvider : IOutputCacheKeysProvider
    {
        public string GetCacheProfileCacheKey(HttpRequest request, string forPath = null, string httpMethod = null)
        {
            return $"{httpMethod ?? request.Method}_{request.Host}{forPath ?? request.Path}";
        }

        public string GetRequestCacheKey(HttpContext context, OutputCacheProfile profile, string forPath = null, string httpMethod  = null)
        {
            HttpRequest request = context.Request;
            string key = GetCacheProfileCacheKey(request, forPath, httpMethod) + "_";

            if (!string.IsNullOrEmpty(profile.VaryByParam))
            {
                foreach (string param in profile.VaryByParam.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (param == "*" || request.Query.ContainsKey(param))
                    {
                        key += param + "=" + request.Query[param];
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
                        key += argument + "=" + varyByCustomService.GetVaryByCustomString(argument);
                    }
                }
            }

            return key.ToLowerInvariant();
        }
    }
}