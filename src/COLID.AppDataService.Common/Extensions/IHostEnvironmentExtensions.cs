using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace COLID.AppDataService.Common.Extensions
{
    public static class IHostEnvironmentExtensions
    {
        public static bool IsLocal(this IHostEnvironment env)
        {
            return env.IsEnvironment("Local");
        }

        public static bool IsOnPrem(this IHostEnvironment env)
        {
            return env.IsEnvironment("OnPrem");
        }

        public static bool IsDocker(this IHostEnvironment env)
        {
            return env.IsEnvironment("Docker");
        }

        public static bool IsTesting(this IHostEnvironment env)
        {
            return env.IsEnvironment("Testing");
        }
    }
}
