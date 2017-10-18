using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace WebEssentials.AspNetCore.OutputCaching
{
    public class OutputCacheResponse
    {
        public OutputCacheResponse(byte[] body, IHeaderDictionary headers)
        {
            Body = body;
            Headers = new Dictionary<string, string>();

            foreach (string name in headers.Keys)
            {
                Headers.Add(name, headers[name]);
            }
        }

        public byte[] Body { get; set; }
        public Dictionary<string, string> Headers { get; }
    }
}