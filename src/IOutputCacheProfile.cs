namespace WebEssentials.AspNetCore.OutputCaching
{
    /// <summary>
    /// An interface to describe a caching profile.
    /// </summary>
    public interface IOutputCacheProfile
    {
        /// <summary>
        /// The duration in seconds of how long to cache the response.
        /// </summary>
        int Duration { get; set; }

        /// <summary>
        /// Comma separated list of HTTP headers to vary the caching by.
        /// </summary>
        string VaryByHeader { get; set; }

        /// <summary>
        /// Comma separated list of query string parameters to vary the caching by.
        /// </summary>
        string VaryByParam { get; set; }
    }
}