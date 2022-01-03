using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebEssentials.AspNetCore.OutputCaching
{
    /// <summary>
    /// An interface for a service to get the VaryByCustom string.
    /// </summary>
    public interface IOutputCacheVaryByCustomService
    {
        /// <summary>
        /// A function that takes an argument and returns a string to vary the caching by.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContext"/>.</param>
        /// <param name="arg">The argument to the VaryByCustom function.</param>
        /// <returns>A string to vary the caching by.</returns>
        string GetVaryByCustomString(HttpContext context, string arg);
    }
}