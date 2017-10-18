using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebEssentials.AspNetCore.OutputCaching
{
    public class OutputCacheMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IOutputCachingService _cache;
        private readonly Func<HttpContext, bool> _filter;

        public OutputCacheMiddleware(RequestDelegate next, IOutputCachingService cache, Func<HttpContext, bool> filter)
        {
            _next = next;
            _cache = cache;
            _filter = filter;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!_filter(context))
            {
                await _next(context);
                return;
            }

            if (_cache.TryGetValue(context.Request.Path, out OutputCacheResponseEntry entry) && entry.IsCached(context, out var item))
            {
                await ServeFromCache(context, item);
            }
            else
            {
                var response = context.Response;
                var originalStream = response.Body;

                using (var ms = new MemoryStream())
                {
                    response.Body = ms;

                    await _next(context);

                    byte[] bytes = ms.ToArray();

                    AddEtagToResponse(context, bytes);
                    AddResponseToCache(context, entry, bytes);

                    ms.Seek(0, SeekOrigin.Begin);

                    await ms.CopyToAsync(originalStream);
                }
            }
        }

        private static async Task ServeFromCache(HttpContext context, OutputCacheResponse value)
        {
            foreach (string name in value.Headers.Keys)
            {
                if (!context.Response.Headers.ContainsKey(name))
                {
                    context.Response.Headers[name] = value.Headers[name];
                }
            }

            if (context.Request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out var etag) && context.Response.Headers[HeaderNames.ETag] == etag)
            {
                context.Response.StatusCode = StatusCodes.Status304NotModified;
            }
            else
            {
                await context.Response.Body.WriteAsync(value.Body, 0, value.Body.Length);
            }
        }

        private void AddResponseToCache(HttpContext context, OutputCacheResponseEntry entry, byte[] bytes)
        {
            if (entry == null)
            {
                var feature = context.Features.Get<OutputCacheFeature>();
                entry = new OutputCacheResponseEntry(context, bytes, feature);

                _cache.Set(context.Request.Path, entry, context);
            }
            else
            {
                entry.Set(context, new OutputCacheResponse(bytes, context.Response.Headers));
            }
        }

        private static void AddEtagToResponse(HttpContext context, byte[] bytes)
        {
            if (context.Response.StatusCode != StatusCodes.Status200OK)
                return;

            if (!context.IsOutputCachingEnabled(out var feature))
                return;

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