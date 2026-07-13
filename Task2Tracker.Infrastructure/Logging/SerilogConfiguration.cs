using Microsoft.Extensions.Configuration;
using Serilog;

namespace Task2Tracker.Infrastructure.Logging;

public static class SerilogConfiguration
{
    public static void Configure(
        IConfiguration configuration,
        LoggerConfiguration loggerConfiguration)
    {
        loggerConfiguration
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext();
    }
}