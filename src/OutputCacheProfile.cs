namespace WebEssentials.AspNetCore.OutputCaching
{
    /// <summary>
    /// An implementation of an <seealso cref="IOutputCacheProfile"/>.
    /// </summary>
    public class OutputCacheProfile : IOutputCacheProfile
    {
        /// <summary>
        /// The duration in seconds of how long to cache the response.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Comma separated list of HTTP headers to vary the caching by.
        /// </summary>
        public string VaryByHeader { get; set; }

        /// <summary>
        /// Comma separated list of query string parameters to vary the caching by.
        /// </summary>
        public string VaryByParam { get; set; }
    }
}
