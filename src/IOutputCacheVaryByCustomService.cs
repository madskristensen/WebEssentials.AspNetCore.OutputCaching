using System.Threading.Tasks;

namespace WebEssentials.AspNetCore.OutputCaching
{
    public interface IOutputCacheVaryByCustomService
    {
        string GetVaryByCustomString(string arg);
    }
}