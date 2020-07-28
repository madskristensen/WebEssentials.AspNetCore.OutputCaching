using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace WebEssentials.AspNetCore.OutputCaching
{
    internal class OutputCacheKeysProvider : IOutputCacheKeysProvider
    {
        public string GetCacheProfileCacheKey(HttpRequest request)
        {
            return $"{request.Method}_{request.Host}{request.Path}";
        }

        public string GetRequestCacheKey(HttpContext context, OutputCacheProfile profile, JObject jObject)
        {
            HttpRequest request = context.Request;
            string key = GetCacheProfileCacheKey(request) + "_";

            if (!string.IsNullOrEmpty(profile.VaryByParam))
            {
                key = profile.VaryByParam.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Where(param =>
                        param == "*" || request.Query.ContainsKey(param) || (jObject != null && jObject.ContainsKey(param)))
                    .Aggregate(key,
                        (current, param) =>
                            current + (jObject != null
                                ? param + "=" + jObject.SelectToken(param)
                                : param + "=" + request.Query[param]));
            }

            if (!string.IsNullOrEmpty(profile.VaryByHeader))
            {
                key = profile.VaryByHeader.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Where(header => header == "*" || request.Headers.ContainsKey(header)).Aggregate(key,
                        (current, header) => current + (header + "=" + request.Headers[header]));
            }

            if (!string.IsNullOrEmpty(profile.VaryByCustom))
            {
                IOutputCacheVaryByCustomService varyByCustomService = context.RequestServices.GetService<IOutputCacheVaryByCustomService>();

                if (varyByCustomService != null)
                {
                    key = profile.VaryByCustom.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Aggregate(key,
                        (current, argument) =>
                            current + (argument + "=" + varyByCustomService.GetVaryByCustomString(argument)));
                }
            }

            if (context.User.Identity.IsAuthenticated)
            {
                key += "UserId=" + context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            }

            return key;
        }
    }
}