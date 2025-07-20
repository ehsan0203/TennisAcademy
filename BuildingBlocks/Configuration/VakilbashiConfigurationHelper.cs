using Microsoft.Extensions.Configuration;
using Serilog;

namespace BuildingBlocks.Configuration
{
    public static class Logging
    {
        public static void AddLogging(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}