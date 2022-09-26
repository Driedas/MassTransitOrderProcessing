using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Hosting.OpenTelemetry;

public static class ConfigurationExtensions
{
    public static IHostBuilder AddOpenTelemetry(this IHostBuilder builder, string serviceName)
    {
        return builder.ConfigureServices(services =>
        {
            services.AddOpenTelemetry(serviceName);
        });
    }
    
    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, string serviceName)
    {
        services
            .AddOpenTelemetryTracing(tracing =>
            {
                tracing.AddSource(serviceName);
                tracing.AddSource("MassTransit");
                tracing.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(serviceName));

                tracing.AddProcessor(new ActivityIdProcessor());
                tracing.Configure((container, tracingInner) =>
                    tracingInner.AddProcessor(new UsernameEnrichingProcessor(container)));

                tracing.AddSqlClientInstrumentation(sql =>
                {
                    sql.SetDbStatementForText = false;
                    sql.SetDbStatementForStoredProcedure = true;
                    sql.RecordException = true;
                });
                
                tracing.AddAspNetCoreInstrumentation(asp =>
                {
                    asp.RecordException = true;
                });
                
                tracing.AddHttpClientInstrumentation(http =>
                {
                    http.RecordException = true;
                });

                //options.AddConsoleExporter();

                tracing.AddJaegerExporter();
            });

        return services;
    }
}