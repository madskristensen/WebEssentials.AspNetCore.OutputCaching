using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace WebEssentials.AspNetCore.OutputCaching
{
    internal class OutputCacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOutputCachingService _cache;
        private readonly OutputCacheOptions _options;

        public OutputCacheMiddleware(RequestDelegate next, IOutputCachingService cache, OutputCacheOptions options)
        {
            _next = next;
            _cache = cache;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!_options.DoesRequestQualify(context))
            {
                await _next(context);
            }
            else if (_cache.TryGetValue(_cache.BuildRequestCacheKey(context.Request), out OutputCacheResponseEntry entry) && entry.IsCached(context, out OutputCacheResponse item))
            {
                await ServeFromCacheAsync(context, item);
            }
            else
            {
                await ServeFromMvcAndCacheAsync(context, entry);
            }
        }

        private async Task ServeFromMvcAndCacheAsync(HttpContext context, OutputCacheResponseEntry entry)
        {
            HttpResponse response = context.Response;
            Stream originalStream = response.Body;

            try
            {
                using (var ms = new MemoryStream())
                {
                    response.Body = ms;

                    await _next(context);

                    if (_options.DoesResponseQualify(context))
                    {
                        byte[] bytes = ms.ToArray();

                        AddEtagToResponse(context, bytes);
                        AddResponseToCache(context, entry, bytes);
                    }

                    if (ms.Length > 0)
                    {
                        ms.Seek(0, SeekOrigin.Begin);

                        await ms.CopyToAsync(originalStream);
                    }
                }
            }
            finally
            {
                response.Body = originalStream;
            }
        }

        private static async Task ServeFromCacheAsync(HttpContext context, OutputCacheResponse value)
        {
            // Copy over the HTTP headers
            foreach (string name in value.Headers.Keys)
            {
                if (!context.Response.Headers.ContainsKey(name))
                {
                    context.Response.Headers[name] = value.Headers[name];
                }
            }

            // Serve a conditional GET request when if-none-match header exist
            if (context.Request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out StringValues etag) && context.Response.Headers[HeaderNames.ETag] == etag)
            {
                context.Response.ContentLength = 0;
                context.Response.StatusCode = StatusCodes.Status304NotModified;
            }
            else
            {
                await context.Response.Body.WriteAsync(value.Body, 0, value.Body.Length);
            }
        }

        private void AddResponseToCache(HttpContext context, OutputCacheResponseEntry entry, byte[] bytes)
        {
            if (!context.IsOutputCachingEnabled(out OutputCacheProfile profile))
            {
                return;
            }

            if (entry == null)
            {
                entry = new OutputCacheResponseEntry(context, bytes, profile);
                _cache.Set(_cache.BuildRequestCacheKey(context.Request), entry, context);
            }
            else
            {
                entry.AddResponse(context, new OutputCacheResponse(bytes, context.Response.Headers));
            }
        }

        private static void AddEtagToResponse(HttpContext context, byte[] bytes)
        {
            if (context.Response.StatusCode != StatusCodes.Status200OK)
                return;

            if (!context.IsOutputCachingEnabled(out OutputCacheProfile profile))
            {
                return;
            }

            if (context.Response.Headers.ContainsKey(HeaderNames.ETag))
                return;

            context.Response.Headers[HeaderNames.ETag] = CalculateChecksum(bytes, context.Request);
        }

        private static string CalculateChecksum(byte[] bytes, HttpRequest request)
        {
            byte[] encoding = Encoding.UTF8.GetBytes(request.Headers[HeaderNames.AcceptEncoding].ToString());

            using (var algo = SHA1.Create())
            {
                byte[] buffer = algo.ComputeHash(bytes.Concat(encoding).ToArray());
                return $"\"{WebEncoders.Base64UrlEncode(buffer)}\"";
            }
        }
    }
}