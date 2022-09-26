using Serilog;
using Serilog.Events;

namespace Hosting.Logging;

public static class ConfigurationExtensions
{
    public static LoggerConfiguration Configure(this LoggerConfiguration configuration)
    {
        configuration
            .MinimumLevel.Information()
            .MinimumLevel.Override("MassTransit", LogEventLevel.Debug)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console();

        return configuration;
    }
}