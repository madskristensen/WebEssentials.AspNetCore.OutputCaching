using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;

namespace WebEssentials.AspNetCore.OutputCaching
{
    public class OutputCacheFeature
    {
        public OutputCacheFeature()
        {
            FileDependencies = new List<string>();
            VaryByHeaders = new List<string>(new[] { HeaderNames.AcceptEncoding });
            VaryByParam = new List<string>();
        }

        public List<string> FileDependencies { get; }

        public TimeSpan? SlidingExpiration { get; set; }

        public List<string> VaryByHeaders { get; }

        public List<string> VaryByParam { get; }

        public bool IsEnabled
        {
            get { return SlidingExpiration.HasValue; }
        }
    }
}
