using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace WebEssentials.AspNetCore.OutputCaching
{
    /// <summary>
    /// A version of the route in memory
    /// </summary>
    public class OutputCacheResponse
    {
        /// <summary>
        /// Creates a new instance of the permutation.
        /// </summary>
        public OutputCacheResponse(byte[] body, IHeaderDictionary headers)
        {
            Body = body;
            Headers = new Dictionary<string, string>();

            foreach (string name in headers.Keys)
            {
                Headers.Add(name, headers[name]);
            }
        }

        /// <summary>
        /// The body of the HTTP response.
        /// </summary>
        public byte[] Body { get; set; }

        /// <summary>
        /// The headers of the HTTP response.
        /// </summary>
        public Dictionary<string, string> Headers { get; }
    }
}